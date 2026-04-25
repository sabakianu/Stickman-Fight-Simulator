using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;
    public static event Action<GameState> OnStatechanged;
    public GameObject FightLogo;
    public Timer timer;
    public Panel StrategyPanel;
    public PhaseButton PhaseButton;
    [Header("Players")]
    public GameObject player;
    public GameObject enemy;

    private EnemyAI enemyAI;
    private PlayerScript playerAI;
    private void Awake()
    {
        Instance = this;
        StrategyPanel.HidePanel();

        playerAI = player.GetComponent<PlayerScript>();
        enemyAI = enemy.GetComponent<EnemyAI>();
    }
    private void Update()
    {
        if (State == GameState.Running)
        {
            CheckForKO();
        }
    }

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
                Time.timeScale = 0f; //asta sa oprim pe moment
                Debug.Log("SIMULARE OPRITĂ - KO");
                break;

            default:
                break;
        }

        OnStatechanged?.Invoke(newState);
    }
    private void Start() //aboneaza eventurile
    {
        UpdateGameState(GameState.Start);
        timer.OnTimerFinished += StartNewStrategyPhase;
        PhaseButton.StartRunnig += StartNewRunnigPhase;
    }
    private IEnumerator StartGameCoroutine() //porneste timerul de 2s
    {
        Time.timeScale = 0f;
        FightLogo.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        FightLogo.SetActive(false);
        Time.timeScale = 1f;
        UpdateGameState(GameState.Strategy);
    }

    private void StartNewStrategyPhase()
    {
        UpdateGameState(GameState.Strategy);
    }

    private void StartNewRunnigPhase()
    {
        StrategyPanel.HidePanel();
        Time.timeScale = 1f;

        List<SideAbility> activeMoves = StrategySelectorManager.Instance.GetCurrentDeck();
        playerAI.StartAutoCombat(activeMoves);
        enemyAI.StartEnemyRound();
        UpdateGameState(GameState.Running);
    }

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
public enum GameState
{
    Start,
    Strategy,
    Running,
    End
}