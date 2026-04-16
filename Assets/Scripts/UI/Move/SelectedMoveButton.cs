using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedMoveButton : MonoBehaviour
{
    public Ability ability;
    private Button btn;
    public Button AbilityButton; // butonul de la ce abilitate am selectat
    public TextMeshProUGUI sideIndicator;
    public bool isLeft;

    public event Action UnselectMove;
    public Action handler; //handler pt dezabonare
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(DeleteThisButton);
    }

    public void DeleteThisButton()
    {
        UnselectMove?.Invoke();
        AbilityButton.enabled = true;
        AbilityButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    }
}
