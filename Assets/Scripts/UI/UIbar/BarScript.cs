using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BarScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    public string sliderName;
    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void setMaxAttribute(float val)
    {
        slider.maxValue = val;
        UpdateText();
    }
    public void setAttribute(float val)
    {
        slider.value = val;
        UpdateText();
    }

    public void UpdateText()
    {
        if (text != null)
        {
            string procent = sliderName + ": ";
            procent += (int)slider.value;
            procent += "/";
            procent += (int)slider.maxValue;

            text.text = procent;
        }
    }
}
