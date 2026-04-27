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

    /// <summary>
    /// Seteaza instanta Singleton pentru a permite accesul global din alte scripturi (ex: BodyHitbox)
    /// </summary>
    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Se asigura ca cursorul mouse-ului este vizibil la inceputul scenei
    /// </summary>
    private void Start()
    {
        Cursor.visible = true;
    }

    /// <summary>
    /// Activeaza Tooltip-ul si ii transmite mesajul care trebuie afisat
    /// </summary>
    /// <param name="message">Continutul textului preluat de obicei din BodyPartData</param>
    public void Show(string message)
    {
        ToolTip.setMessage(message);
        ToolTip.Toggle(true);
    }

    /// <summary>
    /// Dezactiveaza Tooltip-ul si curata continutul acestuia
    /// </summary>
    public void Hide()
    {
        ToolTip.Empty();
        ToolTip.Toggle(false);
    }

}
