using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum AttackSide
{
    Right = 0,
    Left = 1,
    Central = 2

}
public enum RelativeSide
{
    SameSide,
    OppositeSide
}
public enum AbilityType
{
    Attack,
    Defense,
    Dodge
}

[System.Serializable]
public class BodyPartRequirement
{
    public string partName;
    public BodyZone zone;
    public RelativeSide relativeSide;
    [Range(0, 1)] public float weight;
}


[System.Serializable]
public class JointDamageTarget
{
    public string jointName;
    public float damagePercent;
}

[System.Serializable]
public class RecoilTarget
{
    public string name;
    public BodyZone zone;
    public RelativeSide relativeSide;
    [Range(0, 1)]
    public float weight;
}

[CreateAssetMenu(fileName = "NewAbility", menuName = "Ability")]
public class Ability : ScriptableObject
{

    public new string name;
    public AbilityType type;

    [Header("Stats")]

    [ShowIf("type", AbilityType.Attack)]
    public float minDamage;

    [ShowIf("type", AbilityType.Attack)]
    public float maxDamage;

    [ShowIf("type", AbilityType.Attack)]
    public float penetration; // cat trece prin muschi/oase etc

    [ShowIf("type", AbilityType.Attack)]
    [Range(0, 1)] public float reflectPercent; // cat se intoarce la atacator

    public List<BodyPartRequirement> muscleRequired = new List<BodyPartRequirement>(); // muschi folositi cu ponderi
    public List<BodyPartRequirement> jointRequired = new List<BodyPartRequirement>(); // joints required
    public List<BodyPartRequirement> boneRequired = new List<BodyPartRequirement>(); // pt dodge oasele

    [ShowIf("type", AbilityType.Dodge)]
    public float dodgePenalty;

    [ShowIf("type", AbilityType.Defense)]
    public float blockValue;

    [Header("Biomechanics")]
    public float baseSpeed = 1.0f; // viteza
    public float energyCost; // cost energie

    [Header("Visuals")]
    public string animatorTrigger;
    public Sprite logo;

    [Header("Targeting")]

    [ShowIf("type", AbilityType.Attack)]
    public BodyZone targetZone;

    [ShowIf("type", AbilityType.Attack)]
    public List<JointDamageTarget> jointTargets;

    [ShowIf("type", AbilityType.Attack)]
    public List<RecoilTarget> recoilTargets = new List<RecoilTarget>(); // unde se duce recoilul
}
public enum BodyZone
{
    Head,
    Torso,
    Arms,
    Legs
}