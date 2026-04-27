using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switch : MonoBehaviour
{
    [SerializeField] private RectTransform handle;

    [Header("Setari Pozitie")]
    [SerializeField] private float offX = -50f;
    [SerializeField] private float onX = 50f;


    private Toggle toggle;

    /// <summary>
    /// Initializeaza componentele si aboneaza metoda de update la evenimentul de toggle
    /// </summary>
    void Awake()
    {
        toggle = GetComponent<Toggle>();

        toggle.onValueChanged.AddListener(OnSwitchChanged);

        UpdateVisuals(toggle.isOn);
    }

    /// <summary>
    /// Metoda de callback care se executa la fiecare apasare a switch-ului
    /// </summary>
    /// <param name="isOn">Starea curenta a toggle-ului</param>
    void OnSwitchChanged(bool isOn)
    {
        UpdateVisuals(isOn);
    }

    /// <summary>
    /// Muta fizic pozitia manerului pe axa X in functie de starea on sau off
    /// </summary>
    /// <param name="isOn">Determina daca manerul trebuie sa fie la pozitia de drapta sau stanga</param>
    void UpdateVisuals(bool isOn)
    {
        float targetX = isOn ? onX : offX;
        handle.anchoredPosition = new Vector2(targetX, 0);
    }
}
