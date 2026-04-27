using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newOrganPartData", menuName = "OrganPartData")]
public class OrganPartData : BodyPartData
{
    [Header("State")]
    [SerializeField] OrganState state = OrganState.Healthy;

    /// <summary>
    /// Returneaza informatiile detaliate despre organ incluzand starea actuala formatata colorat pentru tooltip
    /// </summary>
    /// <returns>Un sir de caractere formatat cu datele organului</returns>
    public override string GetInfo()
    {
        string info = string.Empty;
        info += base.GetInfo();
        info += "<color=#FFD700>State: " + state.ToString() + "</color>\n";

        return info;
    }

    /// <summary>
    /// Reseteaza organul la valorile initiale si starea de sanatate maxima
    /// </summary>
    public override void resetPart()
    {
        base.resetPart();
        state = OrganState.Healthy;
    }

    /// <summary>
    /// Actualizeaza starea organului si modifica drastic ratele de sangerare in functie de pragurile de HP
    /// </summary>
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

    /// <summary>
    /// Gestioneaza diminuarea durerii in timp aplicand o rata de recuperare diferita in functie de gravitatea starii organului
    /// </summary>
    /// <param name="deltaTime">Timpul scurs de la ultimul cadru</param>
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

/// <summary>
/// Defineste starile posibile de sanatate in care se poate afla un organ
/// </summary>
public enum OrganState
{
    Healthy,  // totul functioneaza normal
    Injured,  // organ afectat, performanta redusa
    Critical,  // organ grav afectat, risc KO
    NonFunctional  // organ complet nefunctional
}