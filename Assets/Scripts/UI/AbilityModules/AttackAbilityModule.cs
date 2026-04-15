using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackAbilityModule : AbilityModule
{
    [Header("UI References")]
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] TextMeshProUGUI hitChanceText;
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] TextMeshProUGUI targetZoneText;
    [SerializeField] TextMeshProUGUI musclesText;
    [SerializeField] TextMeshProUGUI jointsText;
    [SerializeField] TextMeshProUGUI energyCost;

    public override void UpdateDisplay(Ability ability, BodyManager playerBody)
    {
        float eficienta = 1.0f;
        float viteza = 1.0f;
        float hitChance = 1f;

        if (playerBody != null)
        {
            eficienta = playerBody.combat.CalculateTotalPower(ability, false);
            hitChance = playerBody.combat.CalculateHitChance(ability, false, null);
            viteza = playerBody.combat.CalculateAttackSpeed(ability, false);
        }

        string mList = "Muscles: " + string.Join(", ", ability.muscleRequired.ConvertAll(r => r.partName));
        string jList = "Joints: " + string.Join(", ", ability.jointRequired.ConvertAll(r => r.partName));

        float dmgMinReal = ability.minDamage * eficienta;
        float dmgMaxReal = ability.maxDamage * eficienta;
        float speedReal = ability.baseSpeed * viteza;

        title.text = ability.name;
        damageText.text = $"Damage: {dmgMinReal:F0}-{dmgMaxReal:F0} (Base: {ability.minDamage}-{ability.maxDamage})";
        hitChanceText.text = "Hit Chance: " + (hitChance * 100f).ToString("F0") + "%";
        speedText.text = "Speed: " + speedReal.ToString("F1");
        targetZoneText.text = "Target: " + ability.targetZone.ToString();
        musclesText.text = mList;
        jointsText.text = jList;
        energyCost.text = "Energy Cost: " + ability.energyCost.ToString();

        Button MuscleBtn = musclesText.GetComponent<Button>();
        Button JointBtn = jointsText.GetComponent<Button>();

        ActivateAdditionalInfo(MuscleBtn, ability, ability.muscleRequired);
        ActivateAdditionalInfo(JointBtn, ability, ability.jointRequired);
    }

}
