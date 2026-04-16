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

    void Awake()
    {
        toggle = GetComponent<Toggle>();

        toggle.onValueChanged.AddListener(OnSwitchChanged);

        UpdateVisuals(toggle.isOn);
    }

    void OnSwitchChanged(bool isOn)
    {
        UpdateVisuals(isOn);
    }

    void UpdateVisuals(bool isOn)
    {
        float targetX = isOn ? onX : offX;
        handle.anchoredPosition = new Vector2(targetX, 0);
    }
}
