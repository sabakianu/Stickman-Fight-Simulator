using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    [Header("Abilities")]
    public List<Ability> baseAbilities; // din ce alegem

    [SerializeField] List<SideAbility> deck; // cele 5 alese


    [Header("Memory")]
    public List<SideAbility> lastPlayerDeck = new List<SideAbility>(); // data trecuta la jucator

    [Header("Panels")]
    [SerializeField] GameObject EnemyAbilityPanel;

    private BodyManager myBody;
    private PlayerScript fighter;

    /// <summary>
    /// Initializeaza referinta catre scriptul de control al luptei atasat inamicului
    /// </summary>
    void Awake()
    {
        fighter = GetComponent<PlayerScript>();
        myBody = GetComponent<BodyManager>();
    }

    /// <summary>
    /// Creeaza un deck format din 5 abilitati unice extrase aleatoriu din lista totala de abilitati
    /// </summary>
    public void ChooseDeck() // face deck ul cu 5 abilitati ccare NU se repeta
    {
        deck.Clear();
        PlayerScript player = FindObjectOfType<PlayerScript>();
        BodyManager playerBody = (player != null) ? player.GetComponent<BodyManager>() : null;

        Debug.Log($"<color=cyan><b>[AI STRATEGY]</b> {gameObject.name} începe alegerea deck-ului...</color>");
        //lista abilitate,scor
        List<(SideAbility move, float score)> scoredPool = new List<(SideAbility, float)>();

        foreach (Ability baseMove in baseAbilities) //abilitatile disponibile
        {
            foreach (bool side in new bool[] { true, false })
            {
                SideAbility variantMove = new SideAbility { ability = baseMove, isLeft = side };
                float currentScore = fighter.CalculateMoveScore(variantMove, myBody, playerBody);
                string debugBonus = "";
                if (lastPlayerDeck != null && lastPlayerDeck.Count > 0)
                {
                    foreach (var pMove in lastPlayerDeck) //ce a avut player
                    {
                        // 
                        if (pMove.ability.type == AbilityType.Attack && variantMove.ability.type == AbilityType.Defense)
                        {
                            if (variantMove.ability.targetZone == pMove.ability.targetZone)
                            {
                                currentScore += 50f; // block pt atac
                                debugBonus += $" | <color=green>Anti-Atac {variantMove.ability.targetZone}: +50</color>";
                            }
                        }
                        if (pMove.ability.type == AbilityType.Defense && variantMove.ability.type == AbilityType.Attack)
                        {
                            if (variantMove.ability.targetZone == pMove.ability.targetZone)
                            {
                                currentScore -= 30f; // player block alta zon de atac
                                debugBonus += $" | <color=red>Evită Gardă {variantMove.ability.targetZone}: -30</color>";
                            }
                        }
                    }
                    if (variantMove.ability.type == AbilityType.Dodge)
                    {
                        int playerAttacks = 0; // nr atacuri
                        foreach (var attackMove in lastPlayerDeck)
                        {

                            if (attackMove.ability.type == AbilityType.Attack)
                                playerAttacks++;
                        }

                        // 20p per attack ability
                        currentScore += playerAttacks * 20f;
                        debugBonus += $" | <color=cyan>DodgeBonus: +{playerAttacks * 20f} ({playerAttacks} atacuri)</color>";
                    }
                }

                // ce i sub -500 nu mai are rost sa punem)
                if (currentScore > -500f)
                {
                    scoredPool.Add((variantMove, currentScore));
                }
            }
        }

        // sort de la cel mai mare
        scoredPool.Sort((a, b) => b.score.CompareTo(a.score));

        string finalSelection = $"<color=yellow><b>[DECK ALES]:</b></color> ";
        // primele 5
        for (int i = 0; i < scoredPool.Count; i++)
        {
            if (deck.Count >= 5) break;

            SideAbility candidate = scoredPool[i].move;

            // sa nu puna 2 carti la fel
            bool alreadyInDeck = deck.Exists(d => d.ability == candidate.ability);

            if (!alreadyInDeck)
            {
                deck.Add(candidate);
                finalSelection += $"[{candidate.ability.name} ({(candidate.isLeft ? "L" : "R")}) - Scor: {scoredPool[i].score:F1}] ";
            }
        }

        Debug.Log(finalSelection);
        ShowEnemyDeck();
    }

    /// <summary>
    /// Lanseaza rutina de lupta automata folosind deck-ul de abilitati generat
    /// </summary>
    public void StartEnemyRound()
    {
        fighter.StartAutoCombat(deck);
    }

    /// <summary>
    /// Actualizeaza iconitele din panoul de interfata al inamicului pentru a reflecta deck-ul curent
    /// </summary>
    private void ShowEnemyDeck()
    {
        int slotCount = EnemyAbilityPanel.transform.childCount;

        for (int i = 0; i < 5; i++)
        {
            Transform displaySlot = EnemyAbilityPanel.transform.GetChild(i);
            Image image = displaySlot.GetComponent<Image>();
            TextMeshProUGUI sideText = displaySlot.GetComponentInChildren<TextMeshProUGUI>(); //textul de side

            if (i < deck.Count)
            {
                image.sprite = deck[i].ability.logo;
                image.color = Color.white;
                sideText.text = deck[i].isLeft ? "L" : "R";
            }
            else
            {
                image.sprite = null;
                image.color = new Color(180f / 255f, 180f / 255f, 180f / 255f, 0.5f);
                sideText.text = "";
            }
        }
    }

    public PlayerScript getFighter()
    {
        return fighter;
    }

    /// <summary>
    /// Salveaza deck ul jucatorului
    /// </summary>
    public void RecordPlayerMoves(List<SideAbility> playerDeck)
    {
        lastPlayerDeck = new List<SideAbility>(playerDeck);
    }
}
