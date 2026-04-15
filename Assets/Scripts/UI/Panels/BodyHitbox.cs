using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BodyHitbox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] BodyPartData bodyPartData;
    private Image img;
    private Gradient gradient;
    private GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;

    void Awake()
    {
        gradient = new Gradient();
        img = GetComponent<Image>();

        // prag minim de alpha (ce e sub asta e ignorat)
        img.alphaHitTestMinimumThreshold = 0.1f;

        //setez culorile pt gradient
        //colorkey (e intre 0 si 1 si pui "points" in gradient unde se afla culorile .time)
        colorKey = new GradientColorKey[5];
        colorKey[0].color = Color.green; colorKey[0].time = 1f; //verde
        colorKey[1].color = Color.yellow; colorKey[1].time = 0.7f; //gelben
        colorKey[2].color = new Color(1f, 0.5f, 0f); colorKey[2].time = 0.4f; //portocaliu
        colorKey[3].color = Color.red; colorKey[3].time = 0.2f; //rosu
        colorKey[4].color = Color.black; colorKey[4].time = 0f; //negru

        //seteam transparenta gradientului (time sa mappezi de unde pana unde)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 0.75f; alphaKey[0].time = 0f;
        alphaKey[1].alpha = 0.75f; alphaKey[1].time = 1f;

        //atasam
        gradient.SetKeys(colorKey, alphaKey);
    }
    void Update()
    {
        float currentHP = bodyPartData.getCurrentHP();
        float maxHP = bodyPartData.getMaxHP();

        //setez procentul (Clamp01 asigura ca e intre 0 si 1)
        float HPprocent = Mathf.Clamp01(currentHP / maxHP);
        img.color = gradient.Evaluate(HPprocent); //ataseaza culoarea corespondenta
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        string bodyPartName = bodyPartData.GetInfo();
        HighlightManager.Instance.Show(bodyPartName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        img.color = Color.white;
        HighlightManager.Instance.Hide();
    }
}
