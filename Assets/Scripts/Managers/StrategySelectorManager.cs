using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Reprezinta o abilitate selectata impreuna cu partea corpului pe care va fi executata
/// </summary>
[System.Serializable]
public class SideAbility
{
    public Ability ability;
    public bool isLeft;
}

public class StrategySelectorManager : MonoBehaviour
{
    public static StrategySelectorManager Instance;

    [Header("Panels")]
    [SerializeField] GameObject AvailableMoves;
    [SerializeField] GameObject CurrentDeck;
    [SerializeField] GameObject PlayerAbilitySelected;
    [SerializeField] GameObject AdditionalInfo;

    [Header("Other UI")]
    [SerializeField] GameObject Displayed_ItemImage;
    [SerializeField] GameObject PinIcon;
    [SerializeField] GameObject ToggleIcon;

    [Header("Toggle")]
    [SerializeField] Toggle toggle;

    private int MaxDeck = 5;
    private int CurrentDeckIndex = 0;

    private AvalabileMoveButton[] buttons;
    [Header("Prefabs")]
    public GameObject SelectedAbilityButton;

    [Header("DysplayElements")]
    [SerializeField] AbilityModule attackModule;
    [SerializeField] AbilityModule dodgeModule;
    [SerializeField] AbilityModule blockModule;

    private int pinnedIndex = -1; // -1 ca nu e nmk pinned , daca da e index

    /// <summary>
    /// Seteaza instanta Singleton pentru acces global
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Initializeaza butoanele disponibile si aboneaza metodele la evenimentele de click, hover si pin
    /// </summary>
    private void Start()
    {
        buttons = AvailableMoves.GetComponentsInChildren<AvalabileMoveButton>();
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].RightClicked += () => TogglePin(index);

            buttons[i].SelectedMove += () =>
        {
            if (AddSelectedMove(index))
            {
                buttons[index].LockVisualState(); //succes
            }
            else
            {
                buttons[index].ResetVisualState(); //deck plin deja
            }
        };


            buttons[i].Hovered += () =>
        {
            if (pinnedIndex != -1 && pinnedIndex != index) //daca avem ceva pinned sa nu schimbam
                return;

            DisplayInfo(index);
        };

            buttons[i].ExitedHovering += () =>
            {
                if (pinnedIndex != -1) //daca avem ceva pinned sa nu scoatem
                    return;

                DeleteInfoDisplayed(index);
            };
        }

        toggle.onValueChanged.AddListener((isOn) =>
        {
            // asta sa isi dea "refresh" cand apaasam toggle
            if (pinnedIndex != -1)
            {
                DisplayInfo(pinnedIndex);
            }
        });
    }

    /// <summary>
    /// Fixeaza sau elibereaza panoul de informatii pentru o anumita abilitate
    /// </summary>
    private void TogglePin(int index)
    {
        if (pinnedIndex == index)
        {
            pinnedIndex = -1;
            PinIcon.SetActive(false);
            AdditionalInfo.SetActive(false);
        }
        else
        {
            pinnedIndex = index;
            PinIcon.SetActive(true);
            DisplayInfo(index);
            AdditionalInfo.SetActive(false);
        }
    }

    /// <summary>
    /// Instantiaza un buton nou in deck-ul curent si configureaza datele abilitatii
    /// </summary>
    private bool AddSelectedMove(int moveIndex) //adauga buton in deck
    {
        if (CurrentDeckIndex >= MaxDeck) return false;

        Transform moveSlotTransform = AvailableMoves.transform.GetChild(moveIndex); // transform e de pozitie si ierarhie
        Transform SelectedButton = moveSlotTransform.transform.GetChild(0);
        AvalabileMoveButton moveData = SelectedButton.GetComponent<AvalabileMoveButton>(); // iau ability din butonul de selectie

        Transform slot = CurrentDeck.transform.GetChild(CurrentDeckIndex);
        GameObject newButton = Instantiate(SelectedAbilityButton, slot); //creez butonul in deck ca si copil al lui slot

        SelectedMoveButton newButtonData = newButton.GetComponent<SelectedMoveButton>(); //selectez butonul creat
        newButtonData.ability = moveData.ability; //bag datele
        newButtonData.isLeft = toggle.isOn; // valoarea toggle ului cand am selectat
        newButtonData.sideIndicator.text = newButtonData.isLeft ? "L" : "R";
        newButtonData.AbilityButton = SelectedButton.GetComponent<Button>();
        newButtonData.GetComponent<Image>().sprite = newButtonData.ability.logo; //setez imaginea

        int index = CurrentDeckIndex;

        newButtonData.handler = () => UnselectMove(index); //creez handler ul
        newButtonData.UnselectMove += newButtonData.handler; //il abonez

        CurrentDeckIndex++;
        return true;
    }

    /// <summary>
    /// Sterge o miscare din deck si reordoneaza restul butoanelor pentru a umple golul
    /// </summary>
    private void UnselectMove(int moveIndex) // sterge buton din deck
    {
        Transform ButtonSlotToDelete = CurrentDeck.transform.GetChild(moveIndex);
        GameObject ButtonToDelete = ButtonSlotToDelete.GetChild(0).gameObject;
        Destroy(ButtonToDelete);

        for (int i = moveIndex + 1; i < CurrentDeckIndex; i++)
        {
            Transform currentSlot = CurrentDeck.transform.GetChild(i);
            Transform previousSlot = CurrentDeck.transform.GetChild(i - 1);

            Transform buttonToMove = currentSlot.GetChild(0);
            SelectedMoveButton SelectedMoveButton = buttonToMove.GetComponent<SelectedMoveButton>();

            SelectedMoveButton.UnselectMove -= SelectedMoveButton.handler; // dezabondez handlerul vechi

            int index = i - 1;
            SelectedMoveButton.handler = () => UnselectMove(index); //creez handler ul
            SelectedMoveButton.UnselectMove += SelectedMoveButton.handler; //il abonez

            buttonToMove.SetParent(previousSlot);
            buttonToMove.localPosition = Vector3.zero;
            buttonToMove.localRotation = Quaternion.identity;
            buttonToMove.localScale = Vector3.one;
            //aici muta si locatia(poz rot scala)
        }
        CurrentDeckIndex--;
    }

    /// <summary>
    /// Activeaza modulul corespunzator (Atac/Dodge/Block) si populeaza datele pentru afisare
    /// </summary>
    private void DisplayInfo(int index)
    {
        Transform moveSlotTransform = AvailableMoves.transform.GetChild(index); // transform e de pozitie si ierarhie
        Transform SelectedButton = moveSlotTransform.transform.GetChild(0);
        AvalabileMoveButton moveData = SelectedButton.GetComponent<AvalabileMoveButton>(); // iau ability din butonul de selectie
        Image img = Instance.Displayed_ItemImage.GetComponent<Image>();

        img.color = new Color(1f, 1f, 1f, 1f); // alb opacitate maxima
        img.sprite = moveData.ability.logo;
        bool isLeft = toggle.isOn;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        BodyManager playerBody = (player != null) ? player.GetComponent<BodyManager>() : null;

        attackModule.gameObject.SetActive(false);
        dodgeModule.gameObject.SetActive(false);
        blockModule.gameObject.SetActive(false);

        if (moveData.ability.type == AbilityType.Attack) // aici panelul in fucntie de abilitate
        {
            attackModule.gameObject.SetActive(true);
            attackModule.UpdateDisplay(moveData.ability, playerBody, isLeft);
        }
        else if (moveData.ability.type == AbilityType.Dodge)
        {
            dodgeModule.gameObject.SetActive(true);
            dodgeModule.UpdateDisplay(moveData.ability, playerBody, isLeft);
        }
        else if (moveData.ability.type == AbilityType.Defense)
        {
            blockModule.gameObject.SetActive(true);
            blockModule.UpdateDisplay(moveData.ability, playerBody, isLeft); ;
        }

        ToggleIcon.SetActive(true);
    }

    /// <summary>
    /// Curata informatiile afisate si reseteaza aspectul modulelor de info
    /// </summary>
    private void DeleteInfoDisplayed(int Index)
    {
        Image img = Instance.Displayed_ItemImage.GetComponent<Image>();

        img.color = new Color(180f / 255f, 180f / 255f, 180f / 255f, 100f / 255f); // gri opacitate cum am setat
        img.sprite = null;

        attackModule.DeleteInfo();
        dodgeModule.DeleteInfo();
        blockModule.DeleteInfo();

        ToggleIcon.SetActive(false);
        AdditionalInfo.SetActive(false);
    }

    /// <summary>
    /// Actualizeaza panoul final de abilitati selectate inainte de inceperea luptei
    /// </summary>
    public void ShowSelectedAbilities()
    {
        for (int i = 0; i < 5; i++)
        {
            Transform display = PlayerAbilitySelected.transform.GetChild(i);
            Image image = display.GetComponent<Image>();
            TextMeshProUGUI sideText = display.GetComponentInChildren<TextMeshProUGUI>(); //textul de side
            if (i < CurrentDeckIndex)
            {

                Transform selectedAbilitySlot = CurrentDeck.transform.GetChild(i);
                Transform selectedAbilityButton = selectedAbilitySlot.GetChild(0);

                SelectedMoveButton btnData = selectedAbilityButton.GetComponent<SelectedMoveButton>();

                image.color = new Color(1f, 1f, 1f, 1f);
                image.sprite = selectedAbilityButton.GetComponent<Image>().sprite;
                sideText.text = btnData.isLeft ? "L" : "R";
            }
            else
            {
                image.sprite = null;
                image.color = new Color(180f / 255f, 180f / 255f, 180f / 255f, 1f);
                sideText.text = "";
            }
        }
    }

    /// <summary>
    /// Returneaza lista de abilitati selectate pentru a fi trimisa catre sistemul de lupta (PlayerScript)
    /// </summary>
    public List<SideAbility> GetCurrentDeck()
    {
        List<SideAbility> chosenAbilities = new List<SideAbility>();
        for (int i = 0; i < CurrentDeck.transform.childCount; i++)
        {
            Transform slot = CurrentDeck.transform.GetChild(i); //ia slotul din deck
            if (slot.childCount > 0)
            {
                SelectedMoveButton btnData = slot.GetChild(0).GetComponent<SelectedMoveButton>();

                SideAbility move = new SideAbility();
                move.ability = btnData.ability; //ia abilitatea prorpiu zisa
                move.isLeft = btnData.isLeft;
                chosenAbilities.Add(move);
            }
        }
        return chosenAbilities;
    }


    /// <summary>
    /// Verifică deck-ul daca are abilitati care nu mai sunt valabile si le scoate 
    /// </summary>
    public void RefreshDeckValidity(BodyManager playerBody)
    {
        if (playerBody == null)
        {
            for (int i = CurrentDeckIndex - 1; i >= 0; i--)
            {
                Transform slot = CurrentDeck.transform.GetChild(i);
                if (slot.childCount > 0)
                {
                    SelectedMoveButton card = slot.GetChild(0).GetComponent<SelectedMoveButton>();

                    if (!playerBody.combat.CanExecuteAbility(card.ability, card.isLeft))
                    {
                        Debug.Log($"Sistem: Executăm 'click' automat pe {card.ability.name}");

                        card.DeleteThisButton();
                    }
                }
            }
        }
    }
}