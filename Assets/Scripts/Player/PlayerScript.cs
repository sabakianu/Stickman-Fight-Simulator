using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public int MaxHP = 100;
    public int HP = 50;
    public bool fightMode = false;

    public GameObject AttackPoint;
    public float radius;
    public LayerMask enemies;

    [SerializeField] DamageSpawner spawner;
    private Coroutine combatRoutine;
    private Animator animator;

    /// <summary>
    /// Initializeaza referinta catre animator la inceputul jocului
    /// </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Actualizeaza starea animatorului in functie de modul de lupta activ
    /// </summary>
    void Update()
    {
        if (fightMode == true)
        {
            animator.SetBool("FightMode", true);

        }
        else
        {
            animator.SetBool("FightMode", false);
        }
    }

    public SideAbility currentMove;

    /// <summary>
    /// Detecteaza inamicii intr-o raza circulara si aplica logica de atac bazata pe sansa de lovire si eficienta muschilor
    /// </summary>
    /// <param name="sideInt">Indexul directiei de atac</param>
    public void attack(int sideInt) //ataca inamicul
    {
        bool isLeft = currentMove.isLeft;

        Collider2D[] enemiesObject = Physics2D.OverlapCircleAll(AttackPoint.transform.position, radius, enemies);
        foreach (Collider2D enemyObject in enemiesObject)
        {
            BodyManager enemyBody = enemyObject.GetComponent<BodyManager>();
            BodyManager myBody = GetComponent<BodyManager>();

            if (enemyBody != null && currentMove != null)
            {

                float hitChance = myBody.combat.CalculateHitChance(currentMove.ability, isLeft, enemyBody); // sansa de a lovi

                if (UnityEngine.Random.value <= hitChance)
                {
                    float myEfficiency = myBody.combat.CalculateTotalPower(currentMove.ability, isLeft); // eficienta muschilor
                    enemyBody.combat.ApplyHitStats(currentMove.ability, isLeft, myEfficiency, myBody); // scadem viata in inamic
                }
                else
                {
                    spawner.AddValues("Missed", 0);
                    spawner.SpawnPopUp();
                    //Debug.Log($"<color=red>[MISS]</color> {currentMove.name} a ratat din cauza mobilității!");
                }
            }
        }

    }

    /// <summary>
    /// Deseneaza raza de atac in editorul Unity pentru o configurare vizuala mai usoara
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(AttackPoint.transform.position, radius);
    }

    /// <summary>
    /// Porneste procesul automat de lupta si asigura oprirea oricarei rutine anterioare
    /// </summary>
    /// <param name="deck">Lista de abilitati selectate pentru faza de lupta</param>
    public void StartAutoCombat(List<SideAbility> deck)
    {
        StopAutoCombat(); //elimina courutina veche sa facem loc la deck ul curent
        combatRoutine = StartCoroutine(CombatRoutine(deck));

    }

    public void StopAutoCombat()
    {
        if (combatRoutine != null)
        {
            StopCoroutine(combatRoutine);
            combatRoutine = null;
        }
    }

    /// <summary>
    /// Corutina care parcurge deck-ul de abilitati si le executa in functie de tip (Atac, Dodge, Defense) si stamina disponibila
    /// </summary>
    private IEnumerator CombatRoutine(List<SideAbility> deck)
    {
        if (deck.Count == 0) yield break;
        BodyManager myBody = GetComponent<BodyManager>();

        float duration = 20f; // asta e pt runnig state
        float timer = 0f;
        float attackCooldown = 2f; // cooldown

        while (timer < duration)
        {
            bool canAttack = deck.Exists(m => myBody.vitals.currentStamina >= m.ability.energyCost);

            if (canAttack)
            {
                // detectam inamicul sa il vedem
                Collider2D hit = Physics2D.OverlapCircle(AttackPoint.transform.position, radius, enemies);
                PlayerScript opponentScript = null;
                BodyManager opponentBody = null;
                SideAbility opponentMove = null;

                if (hit != null)
                {
                    opponentBody = hit.GetComponent<BodyManager>();
                    opponentScript = hit.GetComponent<PlayerScript>();

                    //ce miscare are atunci
                    if (opponentScript != null)
                        opponentMove = opponentScript.currentMove;
                }

                // ce miscare folosim
                currentMove = GetBestMove(opponentMove, deck, myBody, opponentBody);

                if (currentMove.ability.type == AbilityType.Dodge)
                {
                    float dodgePenalty = myBody.combat.CalculateDodgeEffectiveness(currentMove.ability, currentMove.isLeft);
                    myBody.combat.setDodgePenalty(dodgePenalty);

                    animator.SetTrigger(currentMove.ability.animatorTrigger);

                    yield return new WaitForSeconds(attackCooldown);
                    myBody.combat.setDodgePenalty(0f);
                }
                else if (currentMove.ability.type == AbilityType.Defense)
                {
                    myBody.combat.setBlockValue(currentMove.ability.blockValue, currentMove.ability);
                    animator.SetBool(currentMove.ability.animatorTrigger, true);

                    yield return new WaitForSeconds(attackCooldown);
                    animator.SetBool(currentMove.ability.animatorTrigger, false);

                    myBody.combat.setBlockValue(0, null);
                }
                else
                {
                    float speed = myBody.combat.CalculateAttackSpeed(currentMove.ability, currentMove.isLeft);
                    animator.SetFloat("AttackSpeed", currentMove.ability.baseSpeed * speed);

                    // seteaza triggerul pt abilitate
                    animator.SetTrigger(currentMove.ability.animatorTrigger);
                }
            }

            yield return new WaitForSeconds(attackCooldown);
            timer += attackCooldown;
        }
    }

    public float CalculateMoveScore(SideAbility move, BodyManager myBody, BodyManager enemyBody)
    {
        float score = 0;
        float myHealthRatio = myBody.blood.getCurrentHP() / myBody.blood.getMaxHP();

        // ratie stamina
        float currentStamina = myBody.vitals.currentStamina;
        float staminaRatio = currentStamina / myBody.vitals.maxStamina;

        // stamina insuficienta
        if (currentStamina < move.ability.energyCost)
            return -1000f;

        // abilitate nu mai e disponibila
        if (!myBody.combat.CanExecuteAbility(move.ability, move.isLeft))
            return -1000f;

        // stamina e sub 30%: abilitatile scumpe primesc o penalizare (pt economie stamina)
        if (staminaRatio < 0.3f && move.ability.energyCost > 15f)
        {
            // cu cat e mai scumpă abilitatea si mai mica stamina, scorul scade mai mult
            score -= move.ability.energyCost * (1f - staminaRatio) * 2f;
        }

        //cand stamina e mica avem miscari ieftine
        if (staminaRatio < 0.5f && move.ability.energyCost < 10f)
        {
            score += 20f;
        }

        // atac
        if (move.ability.type == AbilityType.Attack)
        {
            float hitChance = myBody.combat.CalculateHitChance(move.ability, move.isLeft, enemyBody);
            score += hitChance * 60f;

            float power = myBody.combat.CalculateTotalPower(move.ability, move.isLeft);
            score += power * 40f;

            BodyZoneContainer targetZone = enemyBody.FindBodyPart(move.ability.targetZone, move.isLeft);
            float zoneHealth = myBody.GetZoneHealthPercent(targetZone);
            score += (1f - zoneHealth) * 50f;

            score *= myHealthRatio;
        }
        // aparare/dodge
        else
        {
            float defenseEff = 0;
            if (move.ability.type == AbilityType.Defense)
                defenseEff = myBody.combat.CalculateBlockEffectiveness(null, 50f, move.ability);
            else if (move.ability.type == AbilityType.Dodge)
                defenseEff = myBody.combat.CalculateDodgeEffectiveness(move.ability, move.isLeft);

            score += defenseEff * 100f;
            score += (1f - myHealthRatio) * 80f;
        }

        score += Random.Range(0f, 10f);
        return score;
    }

    public SideAbility GetBestMove(SideAbility opponentMove, List<SideAbility> hand, BodyManager myBody, BodyManager opponentBody)
    {
        SideAbility winner = null;
        float topScore = -2000f;

        foreach (SideAbility move in hand)
        {
            // scorul abilitatii
            float currentScore = CalculateMoveScore(move, myBody, opponentBody);

            //daca nu e bun , next one
            if (currentScore < -500f)
                continue;

            if (opponentMove != null && opponentMove.ability != null)
            {
                // daca unul ataca, verifica apararea
                if (opponentMove.ability.type == AbilityType.Attack)
                {
                    if (move.ability.type == AbilityType.Defense)
                    {
                        // daca au acelasi targrt block ul
                        if (move.ability.targetZone == opponentMove.ability.targetZone)
                        {
                            currentScore += 130f;
                        }
                    }
                    else if (move.ability.type == AbilityType.Dodge)
                    {
                        //urca dodge ul
                        currentScore += 110f;
                    }

                    // ataca si el
                    if (move.ability.type == AbilityType.Attack)
                    {
                        currentScore += 20f;
                    }
                }

                // daca inamicul se apara
                else if (opponentMove.ability.type == AbilityType.Defense)
                {
                    if (move.ability.type == AbilityType.Attack && move.ability.targetZone == opponentMove.ability.targetZone)
                    {
                        currentScore -= 70f; // penalizare ca apara fix zona aia
                    }
                }
            }

            //zgomot
            currentScore += Random.Range(-10f, 10f);

            // cel mai bun scor
            if (currentScore > topScore)
            {
                topScore = currentScore;
                winner = move;
            }
        }

        return winner;
    }
}


