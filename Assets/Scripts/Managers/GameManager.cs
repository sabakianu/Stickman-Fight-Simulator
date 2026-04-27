using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;
    public static event Action<GameState> OnStatechanged;
    public GameObject FightLogo;
    public GameObject KOPopup;
    public Timer timer;
    public Panel StrategyPanel;
    public PhaseButton PhaseButton;
    [Header("Players")]
    public GameObject player;
    public GameObject enemy;

    private EnemyAI enemyAI;
    private PlayerScript playerAI;

    /// <summary>
    /// Initializeaza singleton-ul si referintele catre scripturile de control ale combatantilor
    /// </summary>
    private void Awake()
    {
        Instance = this;
        StrategyPanel.HidePanel();

        playerAI = player.GetComponent<PlayerScript>();
        enemyAI = enemy.GetComponent<EnemyAI>();
    }

    /// <summary>
    /// Verifica in fiecare cadru daca unul dintre luptatori a fost facut KO in timpul fazei active
    /// </summary>
    private void Update()
    {
        if (State == GameState.Running)
        {
            CheckForKO();
        }
    }

    /// <summary>
    /// Schimba starea curenta a jocului si executa actiunile specifice pentru noua faza (sunete, UI, pauza)
    /// </summary>
    /// <param name="newState">Noua stare in care va trece jocul</param>
    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Start:
                AudioManager.Instance.PlaySound(AudioManager.Instance.Fight);
                StartCoroutine(StartGameCoroutine());
                break;
            case GameState.Strategy:
                enemyAI.ChooseDeck();
                StrategyPanel.ShowPanel();
                Time.timeScale = 0f;
                break;
            case GameState.Running:
                timer.startTimer();
                break;
            case GameState.End:
                StartCoroutine(EndGameSequence());
                break;

            default:
                break;
        }

        OnStatechanged?.Invoke(newState);
    }

    /// <summary>
    /// Aboneaza metodele la evenimentele de timer si de interfata la inceputul jocului
    /// </summary>
    private void Start() //aboneaza eventurile
    {
        UpdateGameState(GameState.Start);
        timer.OnTimerFinished += StartNewStrategyPhase;
        PhaseButton.StartRunnig += StartNewRunnigPhase;
    }

    /// <summary>
    /// Corutina pentru afisarea logo-ului de inceput si tranzitia catre prima faza de strategie
    /// </summary>
    private IEnumerator StartGameCoroutine() //porneste timerul de 2s
    {
        Time.timeScale = 0f;
        FightLogo.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        FightLogo.SetActive(false);
        Time.timeScale = 1f;
        UpdateGameState(GameState.Strategy);
    }

    /// <summary>
    /// Corutina pentru finalizarea meciului, afisarea popup-ului de KO si revenirea la meniul principal
    /// </summary>
    private IEnumerator EndGameSequence()
    {
        timer.StopTimer();
        //playerAI.StopAutoCombat();
        //enemyAI.StopEnemyRound();
        //oprim combatul

        if (KOPopup != null)
        {
            KOPopup.SetActive(true);
        }

        yield return new WaitForSecondsRealtime(3f); // 3 secunde pauza
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    /// <summary>
    /// Callback declansat la expirarea timpului de lupta pentru a reveni la planificare
    /// </summary>
    private void StartNewStrategyPhase()
    {
        UpdateGameState(GameState.Strategy);
    }

    /// <summary>
    /// Porneste faza de lupta propriu-zisa, ascunde interfata si activeaza AI-ul pentru ambii combatanti
    /// </summary>
    private void StartNewRunnigPhase()
    {
        StrategyPanel.HidePanel();
        Time.timeScale = 1f;

        List<SideAbility> activeMoves = StrategySelectorManager.Instance.GetCurrentDeck();
        playerAI.StartAutoCombat(activeMoves);
        enemyAI.StartEnemyRound();
        UpdateGameState(GameState.Running);
    }

    /// <summary>
    /// Interogheaza starea de KO a ambilor luptatori si decide castigatorul meciului
    /// </summary>
    private void CheckForKO()
    {
        BodyManager playerManager = player.GetComponent<BodyManager>();
        BodyManager enemyManager = enemy.GetComponent<BodyManager>();

        bool playerKO = playerManager.isKO;
        bool enemyKO = enemyManager.isKO;

        if (playerKO || enemyKO)
        {
            string winner = playerKO ? "Enemy Wins!" : "Player Wins!";
            Debug.Log("Meci terminat: " + winner);

            UpdateGameState(GameState.End);
        }
    }
}

/// <summary>
/// Defineste starile posibile prin care trece o runda de joc
/// </summary>
public enum GameState
{
    Start,
    Strategy,
    Running,
    End
}