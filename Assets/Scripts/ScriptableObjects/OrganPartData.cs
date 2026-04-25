using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newOrganPartData", menuName = "OrganPartData")]
public class OrganPartData : BodyPartData
{
    [Header("State")]
    [SerializeField] OrganState state = OrganState.Healthy;

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
        state = OrganState.Healthy;
    }
    protected override void checkState()
    {
        if (currentHP <= 0) // daca e 0 e inutilizabil
        {
            state = OrganState.NonFunctional;
            maxBleedRate = 40.0f;
            minBleedRate = 40.0f;
        }
        else if (currentHP < (maxHP * 0.2f)) // prag 20%
        {
            state = OrganState.Critical;
            maxBleedRate = 20.0f;
            minBleedRate = 20.0f;
        }
        else if (currentHP < (maxHP * 0.4f)) // prag 40%
        {
            state = OrganState.Injured;
            maxBleedRate = 10.0f;
            minBleedRate = 10.0f;
        }
    }

    public override void processPainPart(float deltaTime)
    {
        float decayRate = 3.0f;

        switch (state)
        {
            case OrganState.NonFunctional:
                decayRate = 0.0f;
                break;
            case OrganState.Critical:
                decayRate = 0.5f;
                break;
            case OrganState.Injured:
                decayRate = 1.0f;
                break;
        }

        painLevel -= decayRate * deltaTime;
        if (painLevel < 0)
            painLevel = 0;
    }
}

public enum OrganState
{
    Healthy,  // totul functioneaza normal
    Injured,  // organ afectat, performanta redusa
    Critical,  // organ grav afectat, risc KO
    NonFunctional  // organ complet nefunctional
}