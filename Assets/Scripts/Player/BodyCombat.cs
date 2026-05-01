using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCombat : MonoBehaviour
{
    private float blockValue = 0f;
    private float dodgePenalty = 0f;
    private BodyManager body;
    private BodyVitals vitals;

    private Ability activeBlockAbility;

    /// <summary>
    /// Initializeaza referintele catre managerul corpului si sistemul de vitale
    /// </summary>
    void Awake()
    {
        body = GetComponent<BodyManager>();
        vitals = GetComponent<BodyVitals>();
    }

    /// <summary>
    /// Calculeaza si distribuie impactul unei lovituri asupra zonelor anatomice ale tintei
    /// </summary>
    /// <param name="move">Abilitatea folosita</param>
    /// <param name="isLeft">Daca atacul vine de pe partea stanga</param>
    /// <param name="attackerEfficiency">Eficienta atacatorului bazata pe muschi</param>
    /// <param name="attacker">Referinta catre atacator pentru aplicarea reculului</param>
    public void ApplyHitStats(Ability move, bool isLeft, float attackerEfficiency, BodyManager attacker) //afecteaza zonele jucatorului
    {
        BodyZoneContainer target = body.FindBodyPart(move.targetZone, isLeft);
        float rawDamage = Random.Range(move.minDamage, move.maxDamage) * attackerEfficiency;
        float penetration = move.penetration;
        // luam datele zona , damage penetration

        float recoil = rawDamage * move.reflectPercent;
        if (recoil > 0)
        {
            attacker.combat.ApplyRecoil(recoil, move, isLeft);
        }

        if (blockValue > 0 && activeBlockAbility != null && activeBlockAbility.targetZone == move.targetZone)
        {
            // aici pt block
            float mitigation = CalculateBlockEffectiveness(move, rawDamage, activeBlockAbility);

            float absorbedDamage = rawDamage * mitigation;
            ApplyBlockImpact(absorbedDamage, activeBlockAbility);

            rawDamage -= absorbedDamage;
        }

        float damage = Mathf.Max(0, rawDamage); ;

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

    /// <summary>
    /// Aplica daunele in mod egal componentelor dintr-o lista si returneaza daunele care depasesc HP-ul curent
    /// </summary>/// <summary>
    /// Aplica daunele in mod egal componentelor dintr-o lista si returneaza daunele care depasesc HP-ul curent
    /// </summary>
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

    /// <summary>
    /// Distribuie forta de impact a blocajului catre zonele de sustinere ale aparatorului
    /// </summary>
    private void ApplyBlockImpact(float totalImpact, Ability ability)
    {
        foreach (bool side in new bool[] { true, false })
        {
            float sideImpact = totalImpact / 2f;

            foreach (var target in ability.recoilTargets)
            {
                BodyZoneContainer zone = body.GetZoneRequirement(target.zone, target.relativeSide, side);
                if (zone != null)
                {
                    float distributedDamage = sideImpact * target.weight;

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
    }

    /// <summary>
    /// Aplica forta de recul asupra corpului atacatorului in urma executiei unei lovituri
    /// </summary>
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

    /// <summary>
    /// Calculeaza puterea totala a unei abilitati bazata pe starea muschilor si penalizarea de durere
    /// formula: $$Efficiency = \sum (HealthRatio \times Strength \times Weight) \times PainFactor$$
    /// </summary>
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
        float painFactor = 1f - (vitals.GetGlobalPain() / 100f * 0.15f);
        return totalEfficiency * painFactor;
    }

    /// <summary>
    /// Determina viteza finala a atacului influentata de muschi, volum de sange si durere
    /// formula: $$FinalSpeed = (MuscleAvg \times BloodFactor) - PainPenalty$$
    /// </summary>
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

        float totalPain = vitals.GetGlobalPain();
        float painPenalty = totalPain / 100f * 0.25f;
        float finalSpeed = (muscleAverage * bloodFactor) - painPenalty; // viteza finala

        // daca e NaN sau 0 returnam 1 default
        if (float.IsNaN(finalSpeed) || finalSpeed <= 0)
            return 1.0f;

        finalSpeed = Mathf.Round(finalSpeed * 10f) / 10f; // rotunjim

        return Mathf.Max(0.2f, finalSpeed); // nu lasam viteza sub 20%
    }

    /// <summary>
    /// Calculeaza sansa de lovire bazata pe mobilitatea articulatiilor si capacitatea de eschiva a inamicului
    /// </summary>
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

        return finalChance;
    }

    /// <summary>
    /// Calculeaza eficienta eschivei luand in calcul cel mai slab os implicat si nivelul de sange
    /// formula: $$Dodge = (\min(MuscleScore, LowestBone) \times BloodFactor - PainPenalty) \times Penalty$$
    /// </summary>
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

    /// <summary>
    /// Calculeaza cat de mult damage este absorbit de blocaj versus forta atacului
    /// </summary>
    public float CalculateBlockEffectiveness(Ability attack, float attackerDamage, Ability ability)
    {
        float muscleResistance = 0f;
        float boneResistance = 0f;

        if (ability.muscleRequired != null)
        {
            foreach (var req in ability.muscleRequired)
            {
                foreach (bool side in new bool[] { true, false })
                {
                    BodyZoneContainer zone = body.GetZoneRequirement(req.zone, req.relativeSide, side);

                    if (zone != null)
                    {
                        MusclePartData muscle = zone.muscles.Find(m => m != null && m.name.Contains(req.partName));
                        if (muscle != null)
                        {
                            muscleResistance += muscle.getStrength() * muscle.getCurrentDurability() * req.weight;
                        }
                    }
                }
            }
        }

        if (ability.boneRequired != null)
        {
            foreach (var req in ability.boneRequired)
            {
                foreach (bool side in new bool[] { true, false })
                {
                    BodyZoneContainer zone = body.GetZoneRequirement(req.zone, req.relativeSide, side);

                    if (zone != null)
                    {
                        BonePartData bone = zone.bones.Find(b => b != null && b.name.Contains(req.partName));
                        if (bone != null)
                        {
                            boneResistance += bone.getCurrentDurability() * req.weight;
                        }
                    }
                }
            }
        }

        float finalRezistance = (muscleResistance + boneResistance) * (ability.blockValue * 13f); ;
        float eficienta = 1.0f;

        if (attackerDamage > finalRezistance && finalRezistance > 0)
        {
            // block spart
            eficienta = finalRezistance / attackerDamage;
        }

        // DamageFinal = BaseDamage * (1 - (BlockValue * Efficiency))
        float finalBlock = ability.blockValue * eficienta;

        return Mathf.Clamp(finalBlock, 0.05f, 0.95f); // min 5%, max 95% 
    }

    /// <summary>
    /// Verifica daca articulatiile sunt suficient de stabile pentru a permite executia unei miscari
    /// </summary>
    public bool CanExecuteAbility(Ability ability, bool isLeft)
    {
        if (ability.jointRequired == null || ability.jointRequired.Count == 0)
            return true; // caz nu trebuie joints

        float totalStability = 0f;
        float weightSum = 0f;

        foreach (var req in ability.jointRequired)
        {
            BodyZoneContainer zone = body.GetZoneRequirement(req.zone, req.relativeSide, isLeft);
            if (zone != null)
            {
                var joint = zone.joints.Find(j => j != null && j.name.Contains(req.partName));
                if (joint != null)
                {
                    totalStability += joint.getCurrentStability() * req.weight;
                    weightSum += req.weight;
                }
            }
        }

        if (weightSum <= 0)
            return true;

        //putem alege o abilitate peste 30% stabilitate
        return (totalStability / weightSum) >= 0.3f;
    }

    /// <summary>
    /// Elimina din deck miscarile care nu mai pot fi executate din cauza instabilitatii articulatiilor
    /// </summary>
    public void ValidateSelectedMoves(List<SideAbility> selectedDeck)
    {
        selectedDeck.RemoveAll(move =>
        {
            bool isStable = CanExecuteAbility(move.ability, move.isLeft);
            return !isStable;
        });
    }

    public void setBlockValue(float block, Ability ability = null)
    {
        blockValue = block;
        activeBlockAbility = ability;
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
