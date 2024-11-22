using UnityEngine;

[RequireComponent(typeof(ForceReceiver), typeof(InputReader), typeof(NoiseSource))]
public class PlayerStateMachine : StateMachine
{
    [field: Header("Component")]
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public NoiseSource NoiseSource { get; private set; }

    [field: Header("AudioSource")]
    [field: SerializeField] public AudioSource WalkSfx { get; private set; }
    [field: SerializeField] public AudioSource RunSfx { get; private set; }

    [field: Header("Movement")]
    [field: SerializeField] public float WalkingSpeed { get; private set; }
    [field: SerializeField] public float RunningSpeed { get; private set; }
    [field: SerializeField] public float SneakingSpeed { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }

    [field: Header("Collider Value")]
    [field: SerializeField] public float StandingHeight { get; private set; }
    [field: SerializeField] public float StandingOffset { get; private set; }
    [field: SerializeField] public float SneakingHeight { get; private set; }
    [field: SerializeField] public float SneakingOffset { get; private set; }

    [field: Header("Noise")]
    [field: SerializeField] public float WalkingMaxNoise { get; private set; } = 5;
    [field: SerializeField] public float WalkingEmitIntensity { get; private set; } = 2;
    [field: SerializeField] public float RunningMaxNoise { get; private set; } = 10;
    [field: SerializeField] public float RunningEmitIntensity { get; private set; } = 5;
    [field: SerializeField] public float SneakingMaxNoise { get; private set; } = 2;
    [field: SerializeField] public float SneakingEmitIntensity { get; private set; } = 2;

    [field: Header("Status")]
    [field: SerializeField] public bool IsTargetable { get; private set; } = true;

    [field: Header("Victory")]
    [field: SerializeField] public float VictoryScreenWaitingTime { get; private set; } = 2;
    [field: SerializeField] public string[] VictoryAnimationNames { get; private set; } = { "Victory 0", "Victory 1", "Victory 2", "Victory 3" };

    public Transform MainCameraTransform { get; private set; }
    private void Start()
    {
        MainCameraTransform = Camera.main.transform;

        SwitchState(new PlayerFreeLookState(this));
    }

    public void StandUp()
    {
        Controller.center = new Vector3(0, StandingOffset, 0);
        Controller.height = StandingHeight;
    }
    public void Crouch()
    {
        Controller.center = new Vector3(0, SneakingOffset, 0);
        Controller.height = SneakingHeight;
    }

    public void Win()
    {
        IsTargetable = false;
        SwitchState(new PlayerVictoryState(this));
    }

    public void Busted()
    {
        IsTargetable = false;
        SwitchState(new PlayerCollapseState(this));
    }

    public void OnCollapsedAnimationEnd()
    {
        Debug.Log("Game Over.");
        GameManager.Instance?.ChangeState(GameManager.GameState.GameOver);
    }
    public void OnVictoryWaitCompleted()
    {
        Debug.Log("Game Win.");
        GameManager.Instance?.ChangeState(GameManager.GameState.Victory);
    }
}
