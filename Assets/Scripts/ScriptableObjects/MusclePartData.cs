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

    /// <summary>
    /// Compune sirul de informatii pentru tooltip, adaugand datele de oboseala si forta peste cele de baza
    /// </summary>
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

    /// <summary>
    /// Reseteaza muschiul la valorile initiale si starea de sanatate Healthy
    /// </summary>
    public override void resetPart()
    {
        base.resetPart();
        state = MuscleState.Healthy;
    }
    /// <summary>
    /// Verifica HP-ul actual si actualizeaza starea muschiului, durabilitatea si rata de sangerare
    /// </summary>
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

    /// <summary>
    /// Proceseaza scaderea nivelului de durere in timp, influentata de gravitatea leziunii musculare
    /// </summary>
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

    /// <summary>
    /// Returneaza valoarea fortei acestui muschi pentru calculele de damage
    /// </summary>
    public float getStrength()
    {
        return strength;
    }
}

/// <summary>
/// Starile posibile ale unui muschi in functie de integritatea sa
/// </summary>
public enum MuscleState
{

    Healthy,  // totul functioneaza normal
    Injured,  // ranit, performanta redusa
    Teared  // membrul nu mai poate fi folosit

}
