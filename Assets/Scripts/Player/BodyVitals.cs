using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyVitals : MonoBehaviour
{
    [Header("Stamina")]
    public float defaultMaxStamina = 100f;
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    [Header("Stamina Regen")]
    public float currentStaminaRegen = 10f;
    public float defaultStaminaRegen = 10f;

    [Header("UI bars")]
    public BarScript EnergyBar;
    public BarScript PainBar;
    public BarScript ConsciousnessBar;

    private BodyManager body;

    void Awake()
    {
        body = GetComponent<BodyManager>();
    }
    public void processStaminaRegen()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += currentStaminaRegen * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    public bool staminaReq(float cost)
    {
        if (currentStamina >= cost)
        {
            currentStamina -= cost;
            return true;
        }
        return false;
    }

    public float GetGlobalPain()
    {
        float totalPain = 0;
        totalPain += GetZonePain(body.head);
        totalPain += GetZonePain(body.torso);
        totalPain += GetZonePain(body.leftArm);
        totalPain += GetZonePain(body.rightArm);
        totalPain += GetZonePain(body.leftLeg);
        totalPain += GetZonePain(body.rightLeg);
        return totalPain;
    }

    private float GetZonePain(BodyZoneContainer zone)
    {
        float zonePain = 0;
        foreach (var m in zone.muscles)
            if (m != null)
                zonePain += m.getPain();

        foreach (var b in zone.bones)
            if (b != null)
                zonePain += b.getPain();

        return zonePain;
    }

    public void setBars()
    {
        EnergyBar.setMaxAttribute(maxStamina);
        EnergyBar.setAttribute(currentStamina);

        PainBar.setMaxAttribute(body.blood.getMaxHP());
        PainBar.setAttribute(body.blood.getCurrentHP());

        var brain = body.head.organs.Find(o => o != null && o.name.Contains("Brain"));
        if (brain != null)
        {
            ConsciousnessBar.setMaxAttribute(100f);
            ConsciousnessBar.setAttribute(body.GetCurrentConsciousness());
        }
    }
}
