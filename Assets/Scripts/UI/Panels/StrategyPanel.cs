using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private Button StartPhaseButton;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void HidePanel()
    {
        this.gameObject.SetActive(false);
    }
    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
        StartPhaseButton.targetGraphic.color = StartPhaseButton.colors.normalColor;
    }
}