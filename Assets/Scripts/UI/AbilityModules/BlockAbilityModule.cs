using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Modul de interfata responsabil pentru afisarea statisticilor de aparare si blocaj in meniul de strategie
/// </summary>
public class BlockAbilityModule : AbilityModule
{
    [Header("UI References")]
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI blockText;
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] TextMeshProUGUI musclesText;
    [SerializeField] TextMeshProUGUI jointsText;
    [SerializeField] TextMeshProUGUI bonesText;
    [SerializeField] TextMeshProUGUI energyCost;

    /// <summary>
    /// Actualizeaza elementele de UI cu datele despre puterea de blocaj, viteza de executie si cerintele anatomice
    /// </summary>
    /// <param name="ability">Abilitatea de tip blocaj aleasa</param>
    /// <param name="playerBody">Corpul jucatorului folosit pentru calculul vitezei in functie de starea sanatatii</param>
    /// <param name="isLeft">Specifica daca blocajul se face pe partea stanga sau dreapta</param>
    public override void UpdateDisplay(Ability ability, BodyManager playerBody, bool isLeft)
    {
        float viteza = 1.0f;

        if (playerBody != null)
        {
            viteza = playerBody.combat.CalculateAttackSpeed(ability, isLeft);
        }

        string mList = "Muscles: " + string.Join(", ", ability.muscleRequired.ConvertAll(r => r.partName));
        string jList = "Joints: " + string.Join(", ", ability.jointRequired.ConvertAll(r => r.partName));
        string bList = "Bones: " + string.Join(", ", ability.boneRequired.ConvertAll(r => r.partName));

        float speedReal = ability.baseSpeed * viteza;

        title.text = ability.name;
        blockText.text = $"Block: {ability.blockValue * 100:F0}%";
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
    /// Curata toate campurile de text din interfata modulului de blocaj
    /// </summary>
    public override void DeleteInfo()
    {
        title.text = "";
        blockText.text = "";
        speedText.text = "";
        musclesText.text = "";
        jointsText.text = "";
        bonesText.text = "";
        energyCost.text = "";
    }
}
