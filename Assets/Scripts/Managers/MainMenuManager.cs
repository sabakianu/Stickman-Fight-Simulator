using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string combatSceneName = "CombatScene";
    [SerializeField] private string trainingSceneName = "TrainingScene";
    [SerializeField] private string creditsSceneName = "CreditsScene";

    /// <summary>
    /// Incarca scena dedicata luptei propriu-zise
    /// </summary>
    public void PlayCombat()
    {
        SceneManager.LoadScene(combatSceneName);
    }

    /// <summary>
    /// Pregatit pentru incarcarea scenei de antrenament
    /// </summary>
    public void PlayTraining()
    {
        // SceneManager.LoadScene(trainingSceneName);
    }

    /// <summary>
    /// Pregatit pentru afisarea scenei de credite
    /// </summary>
    public void ShowCredits()
    {
        // SceneManager.LoadScene(creditsSceneName);
    }

    /// <summary>
    /// Inchide complet aplicatia (functioneaza doar in varianta de Build a jocului)
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
