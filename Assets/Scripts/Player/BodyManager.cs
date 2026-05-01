using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// fiecare parte a corpului are cate un layer
/// <summary>
/// Container pentru partile anatomice grupate pe zone specifice ale corpului
/// </summary>
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

    /// <summary>
    /// Initializeaza referintele catre modulele de combat si vitale
    /// </summary>
    void Awake()
    {
        combat = GetComponent<BodyCombat>();
        vitals = GetComponent<BodyVitals>();
        isKO = false;
    }

    /// <summary>
    /// Reseteaza corpul si configureaza valorile initiale pentru stamina si UI
    /// </summary>
    void Start()
    {
        resetBody();
        vitals.currentStaminaRegen = vitals.defaultStaminaRegen;
        vitals.currentStamina = vitals.maxStamina;
        vitals.setBars();
    }

    /// <summary>
    /// Bucla principala care proceseaza durerea, sangerarea si efectele organelor in fiecare cadru
    /// </summary>
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

    /// <summary>
    /// Readuce toate partile corpului si volumul de sange la starea initiala
    /// </summary>
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
    /// <summary>
    /// Returneaza containerul corespunzator unei zone si parti specifice
    /// </summary>
    public BodyZoneContainer FindBodyPart(BodyZone zone, bool lookForLeft = true)
    {
        if (zone == BodyZone.Head) return head;
        if (zone == BodyZone.Torso) return torso;

        if (zone == BodyZone.Arms) return lookForLeft ? leftArm : rightArm;
        if (zone == BodyZone.Legs) return lookForLeft ? leftLeg : rightLeg;

        return null;
    }


    /// <summary>
    /// Calculeaza sangerarea totala din toate zonele si scade volumul din rezerva centrala de sange
    /// </summary>
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

    /// <summary>
    /// Reseteaza toate listele de componente dintr-o zona specifica
    /// </summary>
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

    /// <summary>
    /// Proceseaza diminuarea durerii pentru toate componentele dintr-o zona
    /// </summary>
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

    /// <summary>
    /// Calculeaza coagularea si returneaza rata de sangerare cumulata a unei zone
    /// </summary>
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

    /// <summary>
    /// Determina zona tinta in functie de partea atacatorului si relatia (SameSide/OppositeSide)
    /// </summary>
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


    /// <summary>
    /// Aplica penalizari asupra staminei si coagularii in functie de sanatatea inimii, plamanilor si ficatului
    /// </summary>
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

    /// <summary>
    /// Calculeaza nivelul de constienta bazat pe HP-ul creierului, volumul de sange si durerea globala
    /// </summary>
    /// <returns>Valoarea constientei intre 0 si 100</returns>
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

    /// <summary>
    /// Calculeaza procentul de viata dintr o zona anume
    /// </summary>
    public float GetZoneHealthPercent(BodyZoneContainer zone)
    {
        if (zone == null)
            return 1f;

        float currentHP = 0;
        float maxHP = 0;

        foreach (var m in zone.muscles)
        {
            currentHP += m.getCurrentHP();
            maxHP += m.getMaxHP();
        }

        foreach (var b in zone.bones)
        {
            currentHP += b.getCurrentHP();
            maxHP += b.getMaxHP();
        }

        foreach (var j in zone.joints)
        {
            currentHP += j.getCurrentHP();
            maxHP += j.getMaxHP();
        }

        foreach (var o in zone.organs)
        {
            currentHP += o.getCurrentHP();
            maxHP += o.getMaxHP();
        }

        if (maxHP <= 0)
            return 1f;

        return Mathf.Clamp01(currentHP / maxHP);
    }
}
