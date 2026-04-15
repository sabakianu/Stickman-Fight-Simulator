using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCombat : MonoBehaviour
{
    private float blockValue = 0f;
    private float dodgePenalty = 0f;
    private BodyManager body;
    private BodyVitals vitals;
    void Awake()
    {
        body = GetComponent<BodyManager>();
        vitals = GetComponent<BodyVitals>();
    }
    public void ApplyHitStats(Ability move, bool isLeft, float attackerEfficiency, BodyManager attacker) //afecteaza zonele jucatorului
    {
        BodyZoneContainer target = body.FindBodyPart(move.targetZone, isLeft);
        float damage = Random.Range(move.minDamage, move.maxDamage) * attackerEfficiency;
        float penetration = move.penetration;
        // luam datele zona , damage penetration

        float recoil = damage * move.reflectPercent;
        if (recoil > 0)
        {
            attacker.combat.ApplyRecoil(recoil, move, isLeft);
        }

        float remainedDamage = damage * (1f - blockValue);
        damage = remainedDamage;

        //Debug.Log($"[DEBUG] damage dupa: {damage}");

        float jointDamageApplied = 0;
        if (move.jointTargets != null && move.jointTargets.Count > 0)
        {
            foreach (var j in move.jointTargets)
            {
                // cautam jointul respectiv cu numele j.name
                JointPartData joint = target.joints.Find(x => x != null && x.name.Contains(j.jointName));

                // din fiecare joint aplicam procentul de damage si apoi adunam tot damage ul aplicat
                if (joint != null)
                {
                    float jointDamage = damage * j.damagePercent;
                    joint.takeDamage(jointDamage);
                    jointDamageApplied += jointDamage;
                }
            }
        }

        damage = Mathf.Max(0, damage - jointDamageApplied);

        float muscleDamage = damage * (1 - penetration); // o parte in muschi cealalta in oase
        float boneDamage = damage * penetration;
        float organDamage = 0;

        float muscleOverflow = ApplyZoneDamage(target.muscles, muscleDamage);
        body.spawner.AddValues("Muscles", muscleDamage - muscleOverflow);

        boneDamage += muscleOverflow; // adaugam damage ul de la overflow
        float boneOverflow = ApplyZoneDamage(target.bones, boneDamage);
        body.spawner.AddValues("Bones", boneDamage - boneOverflow);

        organDamage += boneOverflow; // adaugam damage ul de la overflow
        ApplyZoneDamage(target.organs, organDamage);
        body.spawner.AddValues("Organs", organDamage);

        body.spawner.SpawnPopUp();
    }

    private float ApplyZoneDamage<T>(List<T> parts, float damage) where T : BodyPartData
    {
        if (parts == null || parts.Count == 0 || damage <= 0) return 0;

        float overflow = 0;
        float partDamage = damage / parts.Count;

        foreach (var part in parts)
        {
            if (part != null)
            {
                float currentHP = part.getCurrentHP();
                part.takeDamage(partDamage);

                if (partDamage > currentHP)
                {
                    overflow += partDamage - currentHP;
                }
            }
        }
        return overflow;
    }

    public void ApplyRecoil(float totalRecoilForce, Ability move, bool isLeft)
    {
        foreach (var target in move.recoilTargets)
        {
            BodyZoneContainer zone = body.GetZoneRequirement(target.zone, target.relativeSide, isLeft);
            if (zone != null)
            {
                float distributedDamage = totalRecoilForce * target.weight;

                var joints = zone.joints.FindAll(j => j != null && j.name.Contains(target.name));
                foreach (var j in joints)
                    j.takeDamage(distributedDamage / joints.Count);

                var muscles = zone.muscles.FindAll(m => m != null && m.name.Contains(target.name));
                foreach (var m in muscles)
                    m.takeDamage(distributedDamage / muscles.Count);

                var bones = zone.bones.FindAll(b => b != null && b.name.Contains(target.name));
                foreach (var b in bones)
                    b.takeDamage(distributedDamage / bones.Count);
            }
        }
    }

    public float CalculateTotalPower(Ability ability, bool isLeft)
    {
        float totalEfficiency = 0f;

        foreach (var req in ability.muscleRequired)
        {
            BodyZoneContainer zone = body.GetZoneRequirement(req.zone, req.relativeSide, isLeft);
            if (zone == null) continue;

            // cautam muschiul
            MusclePartData muscle = zone.muscles.Find(m => m.name.Contains(req.partName));

            if (muscle != null)
            {
                // eficienta = (HP curent / HP max) * strength * pondere
                float healthRatio = muscle.getCurrentHP() / muscle.getMaxHP();
                totalEfficiency += healthRatio * muscle.getStrength() * req.weight;
            }
        }
        return totalEfficiency;
    }

    public float CalculateAttackSpeed(Ability ability, bool isLeft)
    {
        float HPsuma = 0f;
        float totalWeights = 0f;

        if (ability.muscleRequired == null || ability.muscleRequired.Count == 0) // la eroare bagam 1
            return 1.0f;

        foreach (var req in ability.muscleRequired)
        {
            BodyZoneContainer zone = body.GetZoneRequirement(req.zone, req.relativeSide, isLeft); // luam zona folosita
            if (zone != null)
            {
                MusclePartData muscle = zone.muscles.Find(m => m != null && m.name.Contains(req.partName)); //gasim muschiul

                if (muscle != null && muscle.getMaxHP() > 0)
                {
                    float healthRatio = Mathf.Clamp01(muscle.getCurrentHP() / muscle.getMaxHP());
                    HPsuma += healthRatio * req.weight;
                    totalWeights += req.weight;
                } // calculam
            }
        }

        float muscleAverage;
        // daca nu găsește mușchi sau totalWeights e 0, media e 1.0 
        if (totalWeights > 0)
        {
            muscleAverage = HPsuma / totalWeights;
        }
        else
        {
            muscleAverage = 1.0f;
        }

        float bloodFactor = 1.0f;
        if (body.blood != null && body.blood.getMaxHP() > 0)
        {
            float bloodRatio = Mathf.Clamp01(body.blood.getCurrentHP() / body.blood.getMaxHP());
            if (bloodRatio < 0.2f)
            {
                bloodRatio = 0.2f;
            }
            else if (bloodRatio > 1.0f)
            {
                bloodRatio = 1.0f;
            }

            bloodFactor = bloodRatio;
        }

        float finalSpeed = muscleAverage * bloodFactor; // viteza finala

        // daca e NaN sau 0 returnam 1 default
        if (float.IsNaN(finalSpeed) || finalSpeed <= 0)
            return 1.0f;

        finalSpeed = Mathf.Round(finalSpeed * 10f) / 10f; // rotunjim

        return Mathf.Max(0.2f, finalSpeed); // nu lasam viteza sub 20%
    }

    public float CalculateHitChance(Ability move, bool isLeft, BodyManager enemyBody)
    {
        if (move.jointRequired == null || move.jointRequired.Count == 0)
            return 1.0f;

        float totalMobility = 0; // suma mobility * weights
        float totalWeights = 0; // suma weights

        foreach (var req in move.jointRequired)
        {
            BodyZoneContainer zone = body.GetZoneRequirement(req.zone, req.relativeSide, isLeft);

            if (zone != null)
            {
                var joint = zone.joints.Find(j => j != null && j.name.Contains(req.partName));

                if (joint != null)
                {
                    totalMobility += joint.getCurrentMobility() * req.weight;
                    totalWeights += req.weight;
                }
            }
        }

        float rawChance;
        float finalChance;

        if (totalWeights > 0)
        {
            rawChance = totalMobility / totalWeights;// facem media
        }
        else
        {
            rawChance = 1.0f;
        }

        if (enemyBody != null)
        {
            finalChance = rawChance - enemyBody.combat.getDodgePenalty();
        }
        else
        {
            finalChance = rawChance;
        }

        if (finalChance < 0f)
        {
            finalChance = 0f;
        }

        //Debug.Log($"[DEBUG] RAW hit cahnce: {rawChance}");
        //Debug.Log($"[DEBUG] Final hit cahnce: {finalChance}");
        return finalChance;
    }

    public float CalculateDodgeEffectiveness(Ability move, bool isLeft)
    {
        float musclePondere = 0f;
        float totalWeight = 0f;
        float lowestBone = 1f;

        foreach (var req in move.muscleRequired) // suma muschilor si ponderea totala
        {
            BodyZoneContainer zone = body.GetZoneRequirement(req.zone, req.relativeSide, isLeft);

            MusclePartData muscle = zone.muscles.Find(m => m != null && m.name.Contains(req.partName));

            if (muscle != null)
            {
                float muscleHPpercent = Mathf.Clamp01(muscle.getCurrentHP() / muscle.getMaxHP());
                musclePondere += muscleHPpercent * req.weight;
                totalWeight += req.weight;
            }
        }

        foreach (var req in move.boneRequired) // procent minim os
        {
            BodyZoneContainer zone = body.GetZoneRequirement(req.zone, req.relativeSide, isLeft);

            BonePartData bone = zone.bones.Find(b => b != null && b.name.Contains(req.partName));

            if (bone != null)
            {
                float boneHPpercent = Mathf.Clamp01(bone.getCurrentHP() / bone.getMaxHP());
                if (boneHPpercent < lowestBone)
                    lowestBone = boneHPpercent;
            }
        }

        float muscleScore = (totalWeight > 0) ? (musclePondere / totalWeight) : 1f;

        float bloodFactor = 1f;
        if (body.blood != null && body.blood.getMaxHP() > 0)
        {
            bloodFactor = Mathf.Clamp01(body.blood.getCurrentHP() / body.blood.getMaxHP());
        }

        float totalPain = vitals.GetGlobalPain();
        float painPenalty = totalPain / 100f * 0.2f; // 20 la suta din durere la procentaj

        // formula (pe document)
        float finalEffectiveness = (Mathf.Min(muscleScore, lowestBone) * bloodFactor) - painPenalty;
        float finalDodge = finalEffectiveness * move.dodgePenalty;

        return Mathf.Max(0.1f, finalDodge); // lasam din mila minim 10% ferire
    }

    public void setBlockValue(float block)
    {
        blockValue = block;
    }
    public float getBlockValue()
    {
        return blockValue;
    }
    public void setDodgePenalty(float dodge)
    {
        dodgePenalty = dodge;
    }
    public float getDodgePenalty()
    {
        return dodgePenalty;
    }
}
