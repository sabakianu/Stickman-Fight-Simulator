using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvalabileMoveButton : MonoBehaviour, IPointerClickHandler
{
    public Ability ability;
    private Button btn;

    public event Action SelectedMove;
    public event Action Hovered;
    public event Action ExitedHovering;
    public event Action RightClicked;
    private void Start()
    {
        btn = GetComponent<Button>();
        btn.GetComponent<Image>().sprite = ability.logo;
    }
    public void Select()
    {
        SelectedMove?.Invoke();
    }
    public void ResetVisualState()
    {
        btn.enabled = true;
        btn.GetComponent<Image>().color = Color.white; //default alb
    }
    public void LockVisualState()
    {
        btn.enabled = false;
        btn.GetComponent<Image>().color = new Color(218f / 255f, 155f / 255f, 69f / 255f, 1f); //culoare selected
    }
    public void DataDescription()
    {
        Hovered?.Invoke();
    }

    public void DataDescriptionExit()
    {
        ExitedHovering?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClicked?.Invoke();
        }
    }
}
