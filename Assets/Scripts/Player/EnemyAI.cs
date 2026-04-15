using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [Header("Abilities")]
    public List<Ability> abilities;

    [SerializeField] List<Ability> deck;
    [Header("Panels")]
    [SerializeField] GameObject EnemyAbilityPanel;
    private PlayerScript fighter;

    void Awake()
    {
        fighter = GetComponent<PlayerScript>();
    }

    public void ChooseDeck() // face deck ul cu 5 abilitati ccare NU se repeta
    {
        deck.Clear();
        List<Ability> tempPool = new List<Ability>(abilities);

        for (int i = 0; i < 5; i++)
        {
            if (tempPool.Count == 0) break;
            int randomIndex = Random.Range(0, tempPool.Count);
            Ability ability = tempPool[randomIndex];

            deck.Add(ability);
            tempPool.RemoveAt(randomIndex);
        }

        ShowEnemyDeck();
    }

    public void StartEnemyRound()
    {
        fighter.StartAutoCombat(deck);
    }

    private void ShowEnemyDeck()
    {
        int slotCount = EnemyAbilityPanel.transform.childCount;

        for (int i = 0; i < 5; i++)
        {
            Transform displaySlot = EnemyAbilityPanel.transform.GetChild(i);
            Image image = displaySlot.GetComponent<Image>();

            if (i < deck.Count)
            {
                image.sprite = deck[i].logo;
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
