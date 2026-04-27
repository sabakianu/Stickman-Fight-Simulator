using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Clasa de baza abstracta care defineste structura comuna pentru toate modulele de afisare a abilitatilor
/// </summary>
public abstract class AbilityModule : MonoBehaviour
{
    [Header("Expanded Panel")]
    [SerializeField] GameObject InfoPanel;

    protected List<BodyPartRequirement> currentListDisplayed;

    /// <summary>
    /// Metoda abstracta pentru actualizarea informatiilor specifice fiecarui tip de modul (Atac, Eschiva, Blocaj)
    /// </summary>
    /// <param name="ability">Datele abilitatii de afisat</param>
    /// <param name="playerBody">Starea actuala a corpului jucatorului</param>
    /// <param name="isLeft">Daca se calculeaza pentru partea stanga</param>
    public abstract void UpdateDisplay(Ability ability, BodyManager playerBody, bool isLeft);

    /// <summary>
    /// Gestioneaza logica de afisare si ascundere a panoului de detalii suplimentare la apasarea unui buton
    /// </summary>
    /// <param name="btn">Butonul pe care se face click</param>
    /// <param name="ability">Abilitatea curenta</param>
    /// <param name="req">Lista de cerinte anatomice asociate butonului</param>
    protected void ActivateAdditionalInfo(Button btn, Ability ability, List<BodyPartRequirement> req)
    {
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();

            btn.onClick.AddListener(() =>
            {
                if (InfoPanel.activeSelf && currentListDisplayed == req) //caz apas iar pe acelasi
                {
                    InfoPanel.SetActive(false);
                    currentListDisplayed = null; // resetam starea
                }
                else //caz apas buton sau buton nou
                {
                    InfoPanel.SetActive(true);
                    InfoPanel.GetComponent<ExpandedPanel>().Populate(req);
                    currentListDisplayed = req; //ca sa stim daca apasam pe acelasi buton sau nu
                }
            });
        }
    }

    /// <summary>
    /// Metoda abstracta pentru curatarea tuturor campurilor de text si resetarea vizuala a modulului
    /// </summary>
    public abstract void DeleteInfo();
}
