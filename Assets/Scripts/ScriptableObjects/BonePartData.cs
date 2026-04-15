using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newBonePartData", menuName = "BonePartData")]
public class BonePartData : BodyPartData
{
    [Header("State")]
    [SerializeField] BoneState state = BoneState.Healthy;

    public override string GetInfo()
    {
        string info = string.Empty;
        info += base.GetInfo();
        info += "<color=#FFD700>State: " + state.ToString() + "</color>\n";

        return info;
    }

    public override void resetPart()
    {
        base.resetPart();
        state = BoneState.Healthy;
    }
    protected override void checkState()
    {
        if (currentHP <= 0) // daca e 0 einutilizabil
        {
            state = BoneState.Fractured;
            currentDurability = 0;
            maxBleedRate = 15.0f;
            minBleedRate = 5.0f;
        }
        else if (currentHP < (maxHP * 0.4f)) // prag 40%
        {
            state = BoneState.Cracked;
            currentDurability = 0.5f * initialDurability;
            maxBleedRate = 1.0f;
            minBleedRate = 0.1f;
        }
    }

    public override void processPainPart(float deltaTime)
    {
        float decayRate = 3.0f;

        switch (state)
        {
            case BoneState.Fractured:
                decayRate = 0.1f;
                break;
            case BoneState.Cracked:
                decayRate = 1.0f;
                break;
        }

        painLevel -= decayRate * deltaTime;
        if (painLevel < 0)
            painLevel = 0;
    }
}

public enum BoneState
{
    Healthy,  // totul functioneaza normal
    Cracked,  // os partial afectat
    Fractured  // os complet fracturat
}