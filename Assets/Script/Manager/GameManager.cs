using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: Header("States")]
    [field: SerializeField] public GameState CurrentState { get; private set; }
    public bool IsGameEnded { get; private set; }

    public enum GameState
    {
        Exploration,
        Alerted,
        GameOver,
        TimeUp,
        Victory,
        Initialization
    }

    [Header("Components")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject goal;
    [SerializeField] private AudioSource BackgroundMusicExploration;
    [SerializeField] private AudioSource BackgroundMusicAlert;
    [SerializeField] private GameEndUI GameEndScreen;
    [SerializeField] private Clock Clock;

    [Header("Audio Settings")]
    [SerializeField] private float CrossFadeDuration = 1.5f;

    [Header("Spawn Locations")]
    [SerializeField] private List<Transform> PlayerSpawnLocations;
    [SerializeField] private List<Transform> GoalSpawnLocations;

    private int chasingGuardsCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneLoader.Instance?.RegisterInitialization();
        }
        else
        {
            Destroy(gameObject);
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player GameObject not found. Ensure it's tagged as 'Player' or assigned in the inspector.");
            }
        }

        if (goal == null)
        {
            goal = GameObject.Find("Goal Trigger");
            if (goal == null)
            {
                Debug.LogError("Goal Trigger GameObject not found. Ensure it is named 'Goal Trigger' or assigned in the inspector.");
            }
        }
    }

    private void Start()
    {
        CurrentState = GameState.Initialization;
        HandleStateChange();
    }

    public void ChangeState(GameState newState)
    {
        if (IsGameEnded)
        {
            Debug.LogWarning($"Cannot change state from {CurrentState} to {newState}. Game has ended.");
            return;
        }

        if (CurrentState == newState) return;

        CurrentState = newState;
        HandleStateChange();
    }

    private void HandleStateChange()
    {
        if (!IsGameEnded)
        {
            GameEndScreen.ResetComponents();
        }

        switch (CurrentState)
        {
            case GameState.Initialization:
                InitializeGame();
                break;

            case GameState.Exploration:
                HandleExplorationState();
                break;

            case GameState.Alerted:
                HandleAlertedState();
                break;

            case GameState.GameOver:
                EndGameState();
                GameEndScreen.PlayLoseAnimation();
                break;

            case GameState.TimeUp:
                EndGameState();
                player?.GetComponent<PlayerStateMachine>()?.TimeUp();
                GameEndScreen.PlayTimeUpAnimation();
                break;

            case GameState.Victory:
                EndGameState();
                GameEndScreen.PlayWinAnimation();
                break;
        }
    }

    private void InitializeGame()
    {
        if (PlayerSpawnLocations.Count > 0 && player != null)
        {
            Transform randomSpawn = PlayerSpawnLocations[Random.Range(0, PlayerSpawnLocations.Count)];
            player.transform.position = randomSpawn.position;
        }

        if (GoalSpawnLocations.Count > 0 && goal != null)
        {
            Transform randomGoalSpawn = GoalSpawnLocations[Random.Range(0, GoalSpawnLocations.Count)];
            goal.transform.position = randomGoalSpawn.position;
        }

        SceneLoader.Instance?.CompleteInitialization();
        ChangeState(GameState.Exploration);
    }

    private void HandleExplorationState()
    {
        if (BackgroundMusicExploration.isPlaying)
        {
            BackgroundMusicExploration.UnPause();
        }
        else
        {
            CrossFadeAudio(BackgroundMusicAlert, BackgroundMusicExploration);
        }
    }

    private void HandleAlertedState()
    {
        if (BackgroundMusicAlert.isPlaying)
        {
            BackgroundMusicAlert.UnPause();
        }
        else
        {
            CrossFadeAudio(BackgroundMusicExploration, BackgroundMusicAlert);
        }
    }

    private void EndGameState()
    {
        IsGameEnded = true;
        Clock.StopClockRotation();
    }

    private void CrossFadeAudio(AudioSource from, AudioSource to)
    {
        if (from != null && from.isPlaying)
        {
            from.DOFade(0, CrossFadeDuration).OnComplete(() =>
            {
                from.Stop();
                from.volume = 1;
                from.enabled = false;
            });
        }

        if (to != null)
        {
            to.enabled = true;
            to.volume = 0;
            to.Play();
            to.DOFade(1, CrossFadeDuration);
        }
    }

    public void UpdateChasingGuardsCount(int delta)
    {
        if (IsGameEnded) return;

        chasingGuardsCount += delta;

        if (chasingGuardsCount > 0 && CurrentState != GameState.Alerted)
        {
            ChangeState(GameState.Alerted);
        }
        else if (chasingGuardsCount <= 0 && CurrentState != GameState.Exploration)
        {
            chasingGuardsCount = 0;
            ChangeState(GameState.Exploration);
        }
    }
}
