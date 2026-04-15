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
    public float getCurrentHP()
    {
        return currentHP;
    }

    public float getMaxHP()
    {
        return maxHP;
    }

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
    public virtual void resetPart()
    {
        currentHP = maxHP;
        painLevel = 0;
        currentDurability = initialDurability;
        maxBleedRate = 0;
        minBleedRate = 0;
        bleedRate = 0;
    }

    protected virtual void checkState()
    { }

    public virtual void processPainPart(float deltaTime)
    {
        float decayRate = 2.0f;
        painLevel -= decayRate * deltaTime; // deltatime transforma din cadru in secunda pt ca performanta sistemul ar afecta altfel

        if (painLevel < 0)
            painLevel = 0;
    }

    public float getBleedRate()
    {
        return bleedRate;
    }

    public void processBleeding(float deltaTime, float coagulationFactor)
    {
        if (bleedRate <= 0) return;

        bleedRate -= coagulationFactor * deltaTime;

        if (bleedRate < minBleedRate)
        {
            bleedRate = minBleedRate;
        }
    }
    public float getPain()
    {
        return painLevel;
    }
}
