using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Modul de interfata responsabil pentru afisarea si calcularea statisticilor de eschiva in meniul de strategie
/// </summary>
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

    /// <summary>
    /// Actualizeaza toate campurile de text din interfata cu datele calculate despre eficienta eschivei si viteza
    /// </summary>
    /// <param name="ability">Obiectul de tip Ability care contine datele de baza</param>
    /// <param name="playerBody">Referinta catre corpul jucatorului pentru calcularea penalizarilor de damage</param>
    /// <param name="isLeft">Determina daca abilitatea este folosita pe partea stanga sau dreapta</param>
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

    /// <summary>
    /// Reseteaza toate elementele de text la un sir gol pentru a curata interfata
    /// </summary>
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
