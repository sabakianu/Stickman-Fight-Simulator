using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// fiecare parte a corpului are cate un layer
[System.Serializable]
public class BodyZoneContainer
{
    public string name;
    public List<MusclePartData> muscles;
    public List<BonePartData> bones;
    public List<JointPartData> joints;
    public List<OrganPartData> organs;
}


public class BodyManager : MonoBehaviour
{
    [Header("Zones")]
    public BodyZoneContainer head;
    public BodyZoneContainer torso;
    public BodyZoneContainer leftArm;
    public BodyZoneContainer rightArm;
    public BodyZoneContainer leftLeg;
    public BodyZoneContainer rightLeg;
    public BloodPartData blood;


    [Header("Coagulation")]
    public float defaultCoagulation = 0.5f;
    [Header("Damage PopUp")]
    public DamageSpawner spawner;

    [Header("Consciousness")]
    public float consciousness;

    [HideInInspector] public BodyCombat combat;
    [HideInInspector] public BodyVitals vitals;

    [HideInInspector] public bool isKO;

    void Awake()
    {
        combat = GetComponent<BodyCombat>();
        vitals = GetComponent<BodyVitals>();
        isKO = false;
    }
    void Start()
    {
        resetBody();
        vitals.currentStaminaRegen = vitals.defaultStaminaRegen;
        vitals.currentStamina = vitals.maxStamina;
        vitals.setBars();
    }

    void Update()
    {
        float dt = Time.deltaTime;

        processPainZone(head, dt);
        processPainZone(torso, dt);
        processPainZone(leftArm, dt);
        processPainZone(rightArm, dt);
        processPainZone(leftLeg, dt);
        processPainZone(rightLeg, dt);
        BleedSystem(dt);

        vitals.processStaminaRegen();
        ProcessOrganEffects();
        vitals.setBars();

    }


    private void resetBody()
    {
        blood.resetPart();
        resetZone(head);
        resetZone(torso);
        resetZone(leftArm);
        resetZone(rightArm);
        resetZone(leftLeg);
        resetZone(rightLeg);

    }
    // ia zona in care se executa atacul
    public BodyZoneContainer FindBodyPart(BodyZone zone, bool lookForLeft = true)
    {
        if (zone == BodyZone.Head) return head;
        if (zone == BodyZone.Torso) return torso;

        if (zone == BodyZone.Arms) return lookForLeft ? leftArm : rightArm;
        if (zone == BodyZone.Legs) return lookForLeft ? leftLeg : rightLeg;

        return null;
    }



    private void BleedSystem(float dt)
    {
        if (blood == null) return;

        float totalBleedRate = 0;

        totalBleedRate += processBleedingZone(head, dt);
        totalBleedRate += processBleedingZone(torso, dt);
        totalBleedRate += processBleedingZone(leftArm, dt);
        totalBleedRate += processBleedingZone(rightArm, dt);
        totalBleedRate += processBleedingZone(leftLeg, dt);
        totalBleedRate += processBleedingZone(rightLeg, dt);

        // Trimitem rata totală către BloodPartData pentru a scădea volumul
        blood.Bleed(totalBleedRate, dt);
    }
    private void resetZone(BodyZoneContainer zone)
    {
        foreach (var m in zone.muscles)
            if (m != null)
                m.resetPart();
        foreach (var b in zone.bones)
            if (b != null)
                b.resetPart();
        foreach (var j in zone.joints)
            if (j != null)
                j.resetPart();
        foreach (var o in zone.organs)
            if (o != null)
                o.resetPart();
    }

    private void processPainZone(BodyZoneContainer zone, float dt)
    {
        foreach (var m in zone.muscles)
            if (m != null)
                m.processPainPart(dt);

        foreach (var b in zone.bones)
            if (b != null)
                b.processPainPart(dt);

        foreach (var j in zone.joints)
            if (j != null)
                j.processPainPart(dt);

        foreach (var o in zone.organs)
            if (o != null)
                o.processPainPart(dt);
    }

    private float processBleedingZone(BodyZoneContainer zone, float dt)
    {
        float totalZoneBleeding = 0;

        foreach (var m in zone.muscles)
        {
            if (m == null) continue;
            m.processBleeding(dt, blood.coagulationSpeed);
            totalZoneBleeding += m.getBleedRate();
        }

        foreach (var b in zone.bones)
        {
            if (b == null) continue;
            b.processBleeding(dt, blood.coagulationSpeed);
            totalZoneBleeding += b.getBleedRate();
        }

        foreach (var o in zone.organs)
        {
            if (o == null) continue;
            o.processBleeding(dt, blood.coagulationSpeed);
            totalZoneBleeding += o.getBleedRate();
        }

        return totalZoneBleeding;
    }

    public BodyZoneContainer GetZoneRequirement(BodyZone zone, RelativeSide side, bool attackerIsLeft)
    {
        // zona centrala
        if (zone == BodyZone.Head)
            return head;

        if (zone == BodyZone.Torso)
            return torso;


        bool targetIsLeft;
        if (attackerIsLeft) // atacam cu stanga
        {
            targetIsLeft = (side == RelativeSide.SameSide); // sameside = stanga, opposite = dreapta
        }
        else // atacam cu dreapta
        {
            targetIsLeft = (side != RelativeSide.SameSide); // sameside = dreapta, opposite = stanga
        }

        // brate/picioare attackerIsLeft=1=stanga else 0=dreapta
        if (zone == BodyZone.Arms)
            return targetIsLeft ? leftArm : rightArm;

        if (zone == BodyZone.Legs)
            return targetIsLeft ? leftLeg : rightLeg;

        return null;
    }



    private void ProcessOrganEffects()
    {
        // inima
        var heart = torso.organs.Find(o => o != null && o.name.Contains("Heart"));
        if (heart != null)
        {
            float heartPercent = heart.getCurrentHP() / heart.getMaxHP();
            vitals.currentStaminaRegen = vitals.defaultStaminaRegen * heartPercent;
        }

        // plamani
        var lungs = torso.organs.Find(o => o != null && o.name.Contains("Lungs"));
        if (lungs != null)
        {
            float lungsPercent = lungs.getCurrentHP() / lungs.getMaxHP();
            vitals.maxStamina = vitals.defaultMaxStamina * lungsPercent;
            vitals.currentStamina = Mathf.Min(vitals.currentStamina, vitals.maxStamina);
        }

        // ficat
        var liver = torso.organs.Find(o => o != null && o.name.Contains("Liver"));
        if (liver != null && blood != null)
        {
            float liverPercent = liver.getCurrentHP() / liver.getMaxHP();
            blood.coagulationSpeed = defaultCoagulation * liverPercent;
        }

        // creier
        var brain = head.organs.Find(o => o != null && o.name.Contains("Brain"));
        if (brain != null)
        {
            consciousness = GetCurrentConsciousness();

            if (consciousness <= 0.05f)
            {
                isKO = true;
            }
        }
    }


    public float GetCurrentConsciousness()
    {
        var brain = head.organs.Find(o => o != null && o.name.Contains("Brain"));

        float bloodRatio = blood.getCurrentHP() / blood.getMaxHP();
        float bloodPercent;

        if (bloodRatio >= 0.99f)
            bloodPercent = 1f;
        else
            bloodPercent = Mathf.Clamp01((bloodRatio - 0.4f) / 0.6f);

        float fl = brain.getCurrentHP() / brain.getMaxHP() * 100;

        float maxPainTolerance = 1000f;
        float totalPain = vitals.GetGlobalPain();

        float painPenalty = totalPain / maxPainTolerance * 100f;

        float currentC = fl * bloodPercent - painPenalty;

        return Mathf.Clamp(currentC, 0f, 100f);
    }
}
