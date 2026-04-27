using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestioneaza panoul extins care afiseaza lista de cerinte ale partilor corpului pentru o abilitate
/// </summary>
public class ExpandedPanel : MonoBehaviour
{
    [SerializeField] GameObject rowPrefab;
    [SerializeField] Transform container;

    /// <summary>
    /// Sterge elementele vechi din lista si populeaza containerul cu noile cerinte primite
    /// </summary>
    /// <param name="req">Lista de cerinte de tip BodyPartRequirement</param>
    public void Populate(List<BodyPartRequirement> req)
    {
        for (int i = 1; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        } // curatam lista veche

        foreach (var r in req)
        {
            CreateRow(r);
        }
    }

    /// <summary>
    /// Instantiaza un rand nou in interfata si completeaza textele cu numele partii, partea relativa si ponderea
    /// </summary>
    /// <param name="req">Obiectul de tip cerinta care contine datele necesare afisarii</param>
    private void CreateRow(BodyPartRequirement req)
    {
        GameObject row = Instantiate(rowPrefab, container);

        TextMeshProUGUI nameTxt = row.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI sideTxt = row.transform.Find("Side").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI weightTxt = row.transform.Find("Weight").GetComponent<TextMeshProUGUI>();

        nameTxt.text = req.partName;
        if (req.relativeSide == RelativeSide.SameSide)
        {
            sideTxt.text = "Same";
        }
        else
        {
            sideTxt.text = "Opposite";
        }

        float percent = req.weight * 100f;
        weightTxt.text = percent.ToString() + "%";
    }
}
