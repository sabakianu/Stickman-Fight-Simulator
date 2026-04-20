using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(AttackPoint.transform.position, radius);
    }

    public void StartAutoCombat(List<SideAbility> deck)
    {
        if (combatRoutine != null) //elimina courutina veche sa facem loc la deck ul curent
        {
            StopCoroutine(combatRoutine);
        }
        combatRoutine = StartCoroutine(CombatRoutine(deck));

    }

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
                // miscare random
                int randomIndex = UnityEngine.Random.Range(0, deck.Count);
                currentMove = deck[randomIndex];

                while (myBody.vitals.staminaReq(currentMove.ability.energyCost) == false)
                {
                    randomIndex = UnityEngine.Random.Range(0, deck.Count);
                    currentMove = deck[randomIndex];
                }

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
}


