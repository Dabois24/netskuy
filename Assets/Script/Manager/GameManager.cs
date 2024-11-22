using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: Header("States")]
    [field: SerializeField] public GameState CurrentState { get; private set; }

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

    [Header("Audio Settings")]
    [SerializeField] private float CrossFadeDuration = 1.5f;

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
        if (CurrentState == newState) return;

        CurrentState = newState;
        HandleStateChange();
    }

    private void HandleStateChange()
    {
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
                GameEndScreen.PlayLoseAnimation();
                break;

            case GameState.TimeUp:
                GameEndScreen.PlayTimeUpAnimation();
                break;

            case GameState.Victory:
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
}
