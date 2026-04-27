using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestioneaza acumularea datelor de damage si instantierea ferestrelor de tip popup pe ecran
/// </summary>
public class DamageSpawner : MonoBehaviour
{
    public GameObject PopUp;
    private string text;

    /// <summary>
    /// Initializeaza sirul de text ca fiind gol la pornirea componentei
    /// </summary>
    void Start()
    {
        text = "";
    }

    /// <summary>
    /// Adauga o noua linie de informatii in lista de damage ce urmeaza sa fie afisata
    /// </summary>
    /// <param name="group">Numele partii corpului sau grupului lovit</param>
    /// <param name="number">Valoarea numerica a damage-ului primit</param>
    public void AddValues(string group, float number)
    {
        text += $"{group}: {Mathf.RoundToInt(number)}\n";
    }

    /// <summary>
    /// Creeaza fizic obiectul de popup in scena si ii transmite textul acumulat dupa care il reseteaza
    /// </summary>
    public void SpawnPopUp()
    {
        if (PopUp == null)
            return;

        // copilul spawnerului
        GameObject instance = Instantiate(PopUp, transform);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        DamagePopUp script = instance.GetComponent<DamagePopUp>();
        if (script != null)
        {
            script.putNumber(text);
            text = "";
        }
    }
}
