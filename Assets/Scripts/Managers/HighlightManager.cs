using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEditor.VersionControl;

public class HighlightManager : MonoBehaviour
{
    public static HighlightManager Instance;

    [SerializeField] Tooltip ToolTip;
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Cursor.visible = true;
    }

    private void Update()
    {

    }

    public void Show(string message)
    {
        ToolTip.setMessage(message);
        ToolTip.Toggle(true);
    }

    public void Hide()
    {
        ToolTip.Empty();
        ToolTip.Toggle(false);
    }

}
