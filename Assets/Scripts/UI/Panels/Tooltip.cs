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

    public void Toggle(bool state)
    {
        Poz.gameObject.SetActive(state);
    }

    public void setMessage(string message)
    {
        BodyPartInfo.text = message;
    }

    public void Empty()
    {
        BodyPartInfo.text = string.Empty;
    }

    private void LeftPivot()
    {
        Poz.pivot = new Vector2(0f, Poz.pivot.y);
    }

    private void RightPivot()
    {
        Poz.pivot = new Vector2(1f, Poz.pivot.y);
    }

    private void DownPivot()
    {
        Poz.pivot = new Vector2(Poz.pivot.x, 1f);
    }

    private void UpPivot()
    {
        Poz.pivot = new Vector2(Poz.pivot.x, 0f);
    }

    public void SetBoneSize()
    {
        Poz.sizeDelta = boneSize;
    }
    public void SetMuscleSize()
    {
        Poz.sizeDelta = muscleSize;
    }
    public void SetJointOrganSize()
    {
        Poz.sizeDelta = joint_organsSize;
    }
}
