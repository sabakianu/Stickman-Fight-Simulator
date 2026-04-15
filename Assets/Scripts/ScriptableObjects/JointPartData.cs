using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newJointPartData", menuName = "JointPartData")]
public class JointPartData : BodyPartData
{
    [Header("Mobility")]
    [SerializeField] float initialMobility;
    [SerializeField] float currentMobility;
    [Header("Stability")]
    [SerializeField] float initialStability;
    [SerializeField] float currentStability;
    [Header("State")]
    [SerializeField] JointState state = JointState.Healthy;

    public override string GetInfo()
    {
        string info = string.Empty;
        info += base.GetInfo();
        info += "<color=#3CB371>Mobility: " + currentMobility + "</color>\n";
        info += "<color=#3CB371>Mobility: " + currentStability + "</color>\n";
        info += "<color=#FFD700>State: " + state.ToString() + "</color>\n";

        return info;
    }
    public override void resetPart()
    {
        base.resetPart();
        currentMobility = initialMobility;
        currentStability = initialStability;
        state = JointState.Healthy;
    }
    protected override void checkState()
    {
        if (currentHP <= 0) // daca e 0 einutilizabil
        {
            state = JointState.Dislocated;
            currentDurability = 0.1f;
            currentMobility = initialMobility * 0.1f;
            currentStability = initialStability * 0.1f;
        }
        else if (currentHP < (maxHP * 0.5f)) // prag 50%
        {
            state = JointState.Sprained;
            currentDurability = 0.5f * initialDurability;
            currentMobility = initialMobility * 0.5f;
            currentStability = initialStability * 0.6f;
        }
    }

    public override void processPainPart(float deltaTime)
    {
        float decayRate = 4.0f;

        switch (state)
        {
            case JointState.Dislocated:
                decayRate = 0.3f;
                break;
            case JointState.Sprained:
                decayRate = 1.2f;
                break;
        }

        painLevel -= decayRate * deltaTime;
        if (painLevel < 0)
            painLevel = 0;
    }

    public float getCurrentMobility()
    {
        return currentMobility;
    }
}
public enum JointState
{
    Healthy,  // totul functioneaza normal
    Sprained,  // articulatie intinsa
    Dislocated  // articulatie luxata
}