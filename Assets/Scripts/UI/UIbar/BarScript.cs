using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BarScript : MonoBehaviour
{
    public Slider slider;
    public string sliderName;
    private TextMeshProUGUI text;

    /// <summary>
    /// Cauta componenta de text in copiii obiectului la initializare
    /// </summary>
    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// Seteaza valoarea maxima a slider-ului si actualizeaza textul afisat
    /// </summary>
    /// <param name="val">Valoarea maxima permisa</param>
    public void setMaxAttribute(float val)
    {
        slider.maxValue = val;
        UpdateText();
    }

    /// <summary>
    /// Actualizeaza valoarea curenta a slider-ului si textul aferent
    /// </summary>
    /// <param name="val">Noua valoare curenta</param>
    public void setAttribute(float val)
    {
        slider.value = val;
        UpdateText();
    }

    /// <summary>
    /// Formateaza si afiseaza textul sub forma nume valoare curenta per valoare maxima
    /// </summary>
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
