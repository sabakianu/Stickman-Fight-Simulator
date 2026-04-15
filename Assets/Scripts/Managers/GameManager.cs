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
    public PlayerScript player;
    public EnemyAI enemy;
    private void Awake()
    {
        Instance = this;
        StrategyPanel.HidePanel();
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Start:
                Debug.Log("Start");
                AudioManager.Instance.PlaySound(AudioManager.Instance.Fight);
                StartCoroutine(StartGameCoroutine());
                break;
            case GameState.Strategy:
                Debug.Log("Strategy");
                enemy.ChooseDeck();
                StrategyPanel.ShowPanel();
                Time.timeScale = 0f;
                break;
            case GameState.Running:
                Debug.Log("Running");
                timer.startTimer();
                break;
            case GameState.End:
                Debug.Log("End");
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

        List<Ability> activeMoves = StrategySelectorManager.Instance.GetCurrentDeck();
        player.StartAutoCombat(activeMoves);
        enemy.StartEnemyRound();
        UpdateGameState(GameState.Running);
    }
}
public enum GameState
{
    Start,
    Strategy,
    Running,
    End
}