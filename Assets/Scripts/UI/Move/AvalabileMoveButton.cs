using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Gestioneaza butoanele cu abilitati disponibile din meniul de strategie si starea lor de interactiune
/// </summary>
public class AvalabileMoveButton : MonoBehaviour, IPointerClickHandler
{
    public Ability ability;
    [SerializeField] private Toggle sideToggle;
    [SerializeField] BodyManager body;
    private Button btn;

    /// <summary>
    /// Eveniment declansat la selectarea abilitatii
    /// </summary>
    public event Action SelectedMove;

    /// <summary>
    /// Eveniment declansat cand mouse-ul intra peste buton
    /// </summary>
    public event Action Hovered;

    /// <summary>
    /// Eveniment declansat cand mouse-ul paraseste butonul
    /// </summary>
    public event Action ExitedHovering;

    /// <summary>
    /// Eveniment declansat la apasarea butonului dreapta al mouse-ului
    /// </summary>
    public event Action RightClicked;

    /// <summary>
    /// Initializeaza referinta butonului si logo-ul abilitatii la inceputul jocului
    /// </summary>
    private void Start()
    {
        btn = GetComponent<Button>();
        btn.GetComponent<Image>().sprite = ability.logo;
    }

    /// <summary>
    /// Verifica in timp real daca abilitatea poate fi executata si actualizeaza aspectul vizual al butonului
    /// </summary>
    private void Update()
    {
        if (body == null || ability == null || sideToggle == null)
            return;

        bool isLeft = sideToggle.isOn;
        bool isStable = body.combat.CanExecuteAbility(ability, isLeft);

        btn.interactable = isStable;

        var img = btn.GetComponent<Image>();

        if (btn.interactable)
        {
            if (img.color != new Color(218f / 255f, 155f / 255f, 69f / 255f, 1f))
            {
                img.color = Color.white;
            }
        }
        else
        {
            img.color = new Color(1f, 1f, 1f, 0.3f);
        }
    }

    /// <summary>
    /// Apeleaza evenimentul de selectie a miscarii
    /// </summary>
    public void Select()
    {
        SelectedMove?.Invoke();
    }

    /// <summary>
    /// Reseteaza starea vizuala a butonului la cea implicita
    /// </summary>
    public void ResetVisualState()
    {
        btn.enabled = true;
        btn.GetComponent<Image>().color = Color.white; //default alb
    }

    /// <summary>
    /// Blocheaza butonul si schimba culoarea pentru a indica faptul ca miscarea a fost selectata
    /// </summary>
    public void LockVisualState()
    {
        btn.enabled = false;
        btn.GetComponent<Image>().color = new Color(218f / 255f, 155f / 255f, 69f / 255f, 1f); //culoare selected
    }

    /// <summary>
    /// Declanseaza evenimentul de hover pentru a afisa descrierea abilitatii
    /// </summary>
    public void DataDescription()
    {
        Hovered?.Invoke();
    }

    /// <summary>
    /// Declanseaza evenimentul de iesire pentru a ascunde descrierea abilitatii
    /// </summary>
    public void DataDescriptionExit()
    {
        ExitedHovering?.Invoke();
    }

    /// <summary>
    /// Detecteaza tipul de click si declanseaza evenimentul de RightClicked daca este cazul
    /// </summary>
    /// <param name="eventData">Datele evenimentului de click primite de la Unity</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClicked?.Invoke();
        }
    }
}
