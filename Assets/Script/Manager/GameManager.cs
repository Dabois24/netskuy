using UnityEngine;
using DG.Tweening;

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
        Victory
    }

    [Header("Components")]
    [SerializeField] private AudioSource BackgroundMusicExploration;
    [SerializeField] private AudioSource BackgroundMusicAlert;
    [SerializeField] private GameEndUI GameEndScreen;
    [SerializeField] private Clock Clock;

    [Header("Audio Settings")]
    [SerializeField] private float CrossFadeDuration = 1.5f;

    private int chasingGuardsCount = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
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
            GameEndScreen.ResetComponents();

        switch (CurrentState)
        {
            case GameState.Exploration:
                HandleExplorationState();
                break;

            case GameState.Alerted:
                HandleAlertedState();
                break;

            case GameState.GameOver:
                IsGameEnded = true;
                Clock.StopClockRotation();
                GameEndScreen.PlayLoseAnimation();
                break;

            case GameState.TimeUp:
                IsGameEnded = true;
                Clock.StopClockRotation();
                PlayerStateMachine player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStateMachine>();
                player.TimeUp();
                GameEndScreen.PlayTimeUpAnimation();
                break;

            case GameState.Victory:
                IsGameEnded = true;
                Clock.StopClockRotation();
                GameEndScreen.PlayWinAnimation();
                break;
        }
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
