using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    private TextMeshProUGUI BodyPartInfo;
    private RectTransform Poz;

    //marimile tooltip ului in functie la ce da highlight
    private Canvas canvas;
    private Vector2 boneSize;
    private Vector2 muscleSize;
    private Vector2 joint_organsSize;

    /// <summary>
    /// Initializeaza componentele si seteaza dimensiunile predefinite pentru diferite tipuri de parti ale corpului
    /// </summary>
    private void Awake()
    {
        BodyPartInfo = GetComponentInChildren<TextMeshProUGUI>();
        Poz = GetComponentInChildren<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        Toggle(false);

        //setez marimile
        boneSize = new Vector2(350f, 250f);
        muscleSize = new Vector2(400f, 325f);
        joint_organsSize = new Vector2(375f, 310f);
    }

    /// <summary>
    /// Actualizeaza pozitia dupa mouse si ajusteaza pivotul pentru a ramane in interiorul ecranului
    /// </summary>
    void Update()
    {
        float scale = canvas.scaleFactor;
        Vector2 mousePos = Input.mousePosition;
        float tooltipWidth = Poz.sizeDelta.x * scale;
        float tooltipHeight = Poz.sizeDelta.y * scale;

        if (mousePos.x + tooltipWidth > Screen.width)
            RightPivot();
        else
            LeftPivot();

        // Pivot Y cu buffer
        if (mousePos.y + tooltipHeight > Screen.height)
            DownPivot();
        else
            UpPivot();

        transform.position = mousePos;
    }

    /// <summary>
    /// Activeaza sau dezactiveaza vizibilitatea ferestrei de tooltip
    /// </summary>
    /// <param name="state">Starea de activare dorita</param>
    public void Toggle(bool state)
    {
        Poz.gameObject.SetActive(state);
    }

    /// <summary>
    /// Seteaza mesajul text care va fi afisat in interiorul tooltip-ului
    /// </summary>
    /// <param name="message">Continutul textului</param>
    public void setMessage(string message)
    {
        BodyPartInfo.text = message;
    }

    /// <summary>
    /// Curata continutul textului din tooltip
    /// </summary>
    public void Empty()
    {
        BodyPartInfo.text = string.Empty;
    }

    /// <summary>
    /// Ajusteaza pivotul pe axa X pentru afisare pe partea dreapta a mouse-ului
    /// </summary>
    private void LeftPivot()
    {
        Poz.pivot = new Vector2(0f, Poz.pivot.y);
    }

    /// <summary>
    /// Ajusteaza pivotul pe axa X pentru afisare pe partea stanga a mouse-ului
    /// </summary>
    private void RightPivot()
    {
        Poz.pivot = new Vector2(1f, Poz.pivot.y);
    }

    /// <summary>
    /// Ajusteaza pivotul pe axa Y pentru afisare sub pozitia mouse-ului
    /// </summary>
    private void DownPivot()
    {
        Poz.pivot = new Vector2(Poz.pivot.x, 1f);
    }

    /// <summary>
    /// Ajusteaza pivotul pe axa Y pentru afisare deasupra pozitiei mouse-ului
    /// </summary>
    private void UpPivot()
    {
        Poz.pivot = new Vector2(Poz.pivot.x, 0f);
    }

    /// <summary>
    /// Aplica dimensiunile specifice pentru afisarea datelor despre oase
    /// </summary>
    public void SetBoneSize()
    {
        Poz.sizeDelta = boneSize;
    }

    /// <summary>
    /// Aplica dimensiunile specifice pentru afisarea datelor despre muschi
    /// </summary>
    public void SetMuscleSize()
    {
        Poz.sizeDelta = muscleSize;
    }

    /// <summary>
    /// Aplica dimensiunile specifice pentru afisarea datelor despre organe si articulatii
    /// </summary>
    public void SetJointOrganSize()
    {
        Poz.sizeDelta = joint_organsSize;
    }
}
