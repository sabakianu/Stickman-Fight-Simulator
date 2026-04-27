using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartData : ScriptableObject
{
    [Header("Name")]
    [SerializeField] protected string bodyPartName;
    [Header("HitPoints")]
    [SerializeField] protected float maxHP;
    [SerializeField] protected float currentHP;
    [Header("Pain")]
    [SerializeField] protected float painLevel;
    [SerializeField] protected float painCoefficient;
    [Header("Durability")]
    [SerializeField] protected float initialDurability;
    [SerializeField] protected float currentDurability;
    [Header("Blood")]
    [SerializeField] protected float bleedRate;
    [SerializeField] protected float minBleedRate;
    [SerializeField] protected float maxBleedRate;

    /// <summary>
    /// Returneaza un string formatat cu toate statisticile partii corpului pentru afisarea in tooltip
    /// </summary>
    public virtual string GetInfo()
    {
        string info = string.Empty;
        info += "<color=#00FFFF>Name: " + bodyPartName + "</color>\n";
        info += "<color=red>HP: " + currentHP + '/' + maxHP + "</color>\n";
        info += "<color=#FF0000>Pain: " + painLevel + '/' + "100" + "</color>\n";
        info += "<color=#FF0000>Pain Coefficient: " + painCoefficient + "</color>\n";
        info += "<color=#FF0000>Durability: " + currentDurability + "</color>\n";
        info += "<color=#FF0000>Bleed Rate: " + bleedRate + "</color>\n";
        return info;
    }

    /// <summary>
    /// Returneaza viata curenta a partii corpului
    /// </summary>
    public float getCurrentHP()
    {
        return currentHP;
    }

    /// <summary>
    /// Returneaza viata maxima a partii corpului
    /// </summary>
    public float getMaxHP()
    {
        return maxHP;
    }

    /// <summary>
    /// Proceseaza primirea daunelor, calculeaza reducerea prin durabilitate si creste nivelul de durere si sangerare
    /// </summary>
    /// <param name="damage">Valoarea bruta a damage-ului primit</param>
    public void takeDamage(float damage)
    {
        float realDamage = damage * (1 - currentDurability);
        // formula: realDamage = damageInit *(1-currentDurability)
        //fiindca daca avem 80% durability e defapt 0.8 in float si vom primi 20% din atac

        currentHP = currentHP - realDamage;

        if (currentHP <= 0)
            currentHP = 0;

        bleedRate += realDamage * 0.1f;
        if (bleedRate > maxBleedRate)
            bleedRate = maxBleedRate;

        // formula: (damageInit*paincoef) / (durability +0.1)
        // 0.1 daca durability e 0
        // paincoef cu cat e mai mare cu atat simti mai tare(nervi etc etc)
        float painGained = (damage * painCoefficient) / (currentDurability + 0.1f);
        painLevel += painGained;
        if (painLevel >= 100)
            painLevel = 100;

        checkState();
    }

    /// <summary>
    /// Reseteaza toti parametrii partii corpului la valorile lor initiale de sanatate
    /// </summary>/// <summary>
    /// Reseteaza toti parametrii partii corpului la valorile lor initiale de sanatate
    /// </summary>
    public virtual void resetPart()
    {
        currentHP = maxHP;
        painLevel = 0;
        currentDurability = initialDurability;
        maxBleedRate = 0;
        minBleedRate = 0;
        bleedRate = 0;
    }

    /// <summary>
    /// Metoda protejata ce poate fi suprascrisa pentru a verifica pragurile de stare (fracturi, luxatii, etc)
    /// </summary>
    protected virtual void checkState()
    { }

    /// <summary>
    /// Scade nivelul de durere in mod pasiv in functie de timpul scurs
    /// </summary>
    /// <param name="deltaTime">Timpul scurs intre cadre</param>
    public virtual void processPainPart(float deltaTime)
    {
        float decayRate = 2.0f;
        painLevel -= decayRate * deltaTime; // deltatime transforma din cadru in secunda pt ca performanta sistemul ar afecta altfel

        if (painLevel < 0)
            painLevel = 0;
    }

    /// <summary>
    /// Returneaza rata actuala de sangerare
    /// </summary>
    public float getBleedRate()
    {
        return bleedRate;
    }

    /// <summary>
    /// Gestioneaza procesul de coagulare si oprire a sangerarii in timp
    /// </summary>
    /// <param name="deltaTime">Timpul scurs de la ultimul update</param>
    /// <param name="coagulationFactor">Factorul de vindecare al personajului</param>
    public void processBleeding(float deltaTime, float coagulationFactor)
    {
        if (bleedRate <= 0) return;

        bleedRate -= coagulationFactor * deltaTime;

        if (bleedRate < minBleedRate)
        {
            bleedRate = minBleedRate;
        }
    }

    /// <summary>
    /// Returneaza nivelul actual de durere
    /// </summary>
    public float getPain()
    {
        return painLevel;
    }

    /// <summary>
    /// Returneaza durabilitatea actuala a partii corpului
    /// </summary>
    public float getCurrentDurability()
    {
        return currentDurability;
    }
}
