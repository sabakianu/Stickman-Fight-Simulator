using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestioneaza comportamentul unui buton care reprezinta o miscare deja selectata in lista de actiuni
/// </summary>
public class SelectedMoveButton : MonoBehaviour
{
    public Ability ability;
    private Button btn;
    public Button AbilityButton; // butonul de la ce abilitate am selectat
    public TextMeshProUGUI sideIndicator;
    public bool isLeft;

    /// <summary>
    /// Eveniment declansat atunci cand miscarea este eliminata din lista de selectie
    /// </summary>
    public event Action UnselectMove;
    public Action handler; //handler pt dezabonare

    /// <summary>
    /// Initializeaza referinta catre butonul propriu si adauga ascultatorul pentru evenimentul de click
    /// </summary>
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(DeleteThisButton);
    }

    /// <summary>
    /// Sterge miscarea din lista curenta, anunta sistemul si reactiveaza butonul de selectie original
    /// </summary>
    public void DeleteThisButton()
    {
        UnselectMove?.Invoke();
        AbilityButton.enabled = true;
        AbilityButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    }
}
