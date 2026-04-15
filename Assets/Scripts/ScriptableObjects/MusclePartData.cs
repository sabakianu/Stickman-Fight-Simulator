using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMusclePartData", menuName = "MusclePartData")]
public class MusclePartData : BodyPartData
{
    [Header("Muscle")]
    [SerializeField] float fatigue;
    [SerializeField] float strength;
    [SerializeField] float endurance;
    [Header("State")]
    [SerializeField] MuscleState state = MuscleState.Healthy;

    public override string GetInfo()
    {
        string info = string.Empty;
        info += base.GetInfo();
        info += "<color=#FF4500>Fatigue: " + fatigue + '/' + "100" + "</color>\n";
        info += "<color=#3CB371>Strength: " + strength + "</color>\n";
        info += "<color=#3CB371>Endurance: " + endurance + "</color>\n";
        info += "<color=#FFD700>State: " + state.ToString() + "</color>\n";

        return info;
    }
    public override void resetPart()
    {
        base.resetPart();
        state = MuscleState.Healthy;
    }
    protected override void checkState()
    {
        if (currentHP <= 0) // daca e 0 einutilizabil
        {
            state = MuscleState.Teared;
            currentDurability = 0;
            maxBleedRate = 8.0f;
            minBleedRate = 1.5f;
        }
        else if (currentHP < (maxHP * 0.6f)) // prag 60%
        {
            state = MuscleState.Injured;
            currentDurability = 0.5f * initialDurability;
            maxBleedRate = 0.5f;
            minBleedRate = 0.2f;
        }
    }

    public override void processPainPart(float deltaTime)
    {
        float decayRate = 5.0f;

        switch (state)
        {
            case MuscleState.Teared:
                decayRate = 0.5f;
                break;
            case MuscleState.Injured:
                decayRate = 2.0f;
                break;
        }

        painLevel -= decayRate * deltaTime;
        if (painLevel < 0)
            painLevel = 0;
    }
    public float getStrength()
    {
        return strength;
    }
}

public enum MuscleState
{

    Healthy,  // totul functioneaza normal
    Injured,  // ranit, performanta redusa
    Teared  // membrul nu mai poate fi folosit

}
