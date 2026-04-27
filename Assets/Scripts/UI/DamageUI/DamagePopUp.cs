using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Gestioneaza afisarea vizuala a textului de damage si distrugerea automata a acestuia dupa un timp setat
/// </summary>
public class DamagePopUp : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    public float destroyTime = 0.40f;

    /// <summary>
    /// Programeaza distrugerea obiectului imediat ce acesta a fost spawnat pe scena
    /// </summary>
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    /// <summary>
    /// Seteaza continutul textului care va fi afisat pe ecran
    /// </summary>
    /// <param name="text">Sirul de caractere ce contine cifrele de damage</param>
    public void putNumber(string text)
    {
        if (textMesh != null)
            textMesh.text = text;
    }
}
