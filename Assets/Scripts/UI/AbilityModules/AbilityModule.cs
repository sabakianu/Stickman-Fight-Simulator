using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public abstract class AbilityModule : MonoBehaviour
{
    [Header("Expanded Panel")]
    [SerializeField] GameObject InfoPanel;

    protected List<BodyPartRequirement> currentListDisplayed;
    public abstract void UpdateDisplay(Ability ability, BodyManager playerBody, bool isLeft);

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

    public abstract void DeleteInfo();
}
