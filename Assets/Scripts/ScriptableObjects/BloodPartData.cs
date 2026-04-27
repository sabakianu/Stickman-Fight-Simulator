using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newBloodPartData", menuName = "BloodPartData")]
public class BloodPartData : OrganPartData
{
    [Header("Blood functions")]
    public float coagulationSpeed = 0.5f;
    private float lastRecordedBleedRate;

    /// <summary>
    /// Compune un sir de informatii specific pentru sange afisand volumul actual si rata de pierdere
    /// </summary>
    /// <returns>Sir de caractere formatat pentru tooltip-ul de UI</returns>
    public override string GetInfo()
    {
        string info = string.Empty;
        info += "<color=#00FFFF>Name: " + bodyPartName + "</color>\n";
        info += "<color=red>Volume: " + currentHP + '/' + maxHP + "</color>\n";
        info += "<color=red>Coagulation Speed: " + coagulationSpeed + "</color>\n";
        info += "<color=red>Blood loss: " + lastRecordedBleedRate + "</color>\n";

        return info;
    }

    /// <summary>
    /// Scade volumul de sange in functie de rata totala de sangerare a tuturor partilor corpului
    /// </summary>
    /// <param name="totalBleedRate">Suma ratelor de sangerare de la toate membrele si organele</param>
    /// <param name="deltaTime">Timpul scurs intre update-uri</param>
    public void Bleed(float totalBleedRate, float deltaTime)
    {
        lastRecordedBleedRate = totalBleedRate;

        if (lastRecordedBleedRate <= 0) return;

        float bloodLost = lastRecordedBleedRate * deltaTime;

        currentHP -= bloodLost;

        if (currentHP < 0)
            currentHP = 0;

        checkState();
    }

    /// <summary>
    /// Reseteaza volumul de sange la capacitatea maxima
    /// </summary>
    public override void resetPart()
    {
        base.resetPart();
        currentHP = maxHP;
    }
}
