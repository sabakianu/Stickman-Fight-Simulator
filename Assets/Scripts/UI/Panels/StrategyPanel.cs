using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestioneaza vizibilitatea strategypanel si starea butonului de start
/// </summary>
public class Panel : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private Button StartPhaseButton;

    /// <summary>
    /// Initializeaza referinta catre CanvasGroup si o adauga daca lipseste
    /// </summary>
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// Ascunde panoul prin dezactivarea obiectului
    /// </summary>
    public void HidePanel()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Afiseaza panoul si reseteaza aspectul grafic al butonului de start
    /// </summary>
    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
        StartPhaseButton.targetGraphic.color = StartPhaseButton.colors.normalColor;
    }
}