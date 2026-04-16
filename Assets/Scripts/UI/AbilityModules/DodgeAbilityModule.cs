using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DodgeAbilityModule : AbilityModule
{
    [Header("UI References")]
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI dodgeText;
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] TextMeshProUGUI musclesText;
    [SerializeField] TextMeshProUGUI jointsText;
    [SerializeField] TextMeshProUGUI bonesText;
    [SerializeField] TextMeshProUGUI energyCost;
    public override void UpdateDisplay(Ability ability, BodyManager playerBody, bool isLeft)
    {
        float eficienta = 1.0f;
        float viteza = 1.0f;

        if (playerBody != null)
        {
            eficienta = playerBody.combat.CalculateDodgeEffectiveness(ability, isLeft);
            viteza = playerBody.combat.CalculateAttackSpeed(ability, isLeft);
        }

        string mList = "Muscles: " + string.Join(", ", ability.muscleRequired.ConvertAll(r => r.partName));
        string jList = "Joints: " + string.Join(", ", ability.jointRequired.ConvertAll(r => r.partName));
        string bList = "Bones: " + string.Join(", ", ability.boneRequired.ConvertAll(r => r.partName));

        float dodgeReal = eficienta * 100f;
        float speedReal = ability.baseSpeed * viteza;

        title.text = ability.name;
        dodgeText.text = $"Dodge: {dodgeReal:F0}%";
        speedText.text = "Speed: " + speedReal.ToString("F1");
        musclesText.text = mList;
        jointsText.text = jList;
        bonesText.text = bList;
        energyCost.text = "Energy Cost: " + ability.energyCost.ToString();

        Button MuscleBtn = musclesText.GetComponent<Button>();
        Button JointBtn = jointsText.GetComponent<Button>();
        Button BoneBtn = bonesText.GetComponent<Button>();

        ActivateAdditionalInfo(MuscleBtn, ability, ability.muscleRequired);
        ActivateAdditionalInfo(JointBtn, ability, ability.jointRequired);
        ActivateAdditionalInfo(BoneBtn, ability, ability.boneRequired);
    }

    public override void DeleteInfo()
    {
        title.text = "";
        dodgeText.text = "";
        speedText.text = "";
        musclesText.text = "";
        jointsText.text = "";
        bonesText.text = "";
        energyCost.text = "";
    }
}
