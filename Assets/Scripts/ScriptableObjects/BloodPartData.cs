using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newBloodPartData", menuName = "BloodPartData")]
public class BloodPartData : OrganPartData
{
    [Header("Blood functions")]
    public float coagulationSpeed = 0.5f;
    private float lastRecordedBleedRate;
    public override string GetInfo()
    {
        string info = string.Empty;
        info += "<color=#00FFFF>Name: " + bodyPartName + "</color>\n";
        info += "<color=red>Volume: " + currentHP + '/' + maxHP + "</color>\n";
        info += "<color=red>Coagulation Speed: " + coagulationSpeed + "</color>\n";
        info += "<color=red>Blood loss: " + lastRecordedBleedRate + "</color>\n";

        return info;
    }

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

    public override void resetPart()
    {
        base.resetPart();
        currentHP = maxHP;
    }
}
