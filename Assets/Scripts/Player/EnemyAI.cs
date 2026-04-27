using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [Header("Abilities")]
    public List<SideAbility> abilities;

    [SerializeField] List<SideAbility> deck;
    [Header("Panels")]
    [SerializeField] GameObject EnemyAbilityPanel;
    private PlayerScript fighter;

    /// <summary>
    /// Initializeaza referinta catre scriptul de control al luptei atasat inamicului
    /// </summary>
    void Awake()
    {
        fighter = GetComponent<PlayerScript>();
    }

    /// <summary>
    /// Creeaza un deck format din 5 abilitati unice extrase aleatoriu din lista totala de abilitati
    /// </summary>
    public void ChooseDeck() // face deck ul cu 5 abilitati ccare NU se repeta
    {
        deck.Clear();
        List<SideAbility> tempPool = new List<SideAbility>(abilities);

        for (int i = 0; i < 5; i++)
        {
            if (tempPool.Count == 0) break;
            int randomIndex = Random.Range(0, tempPool.Count);
            SideAbility ability = tempPool[randomIndex];

            deck.Add(ability);
            tempPool.RemoveAt(randomIndex);
        }

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

            if (i < deck.Count)
            {
                image.sprite = deck[i].ability.logo;
                image.color = Color.white;
            }
            else
            {
                image.sprite = null;
                image.color = new Color(180f / 255f, 180f / 255f, 180f / 255f, 0.5f);
            }
        }
    }
}
