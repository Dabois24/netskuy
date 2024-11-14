using UnityEngine;

[RequireComponent(typeof(ForceReceiver), typeof(InputReader))]
public class PlayerStateMachine : StateMachine
{
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }

    [field: SerializeField] public float WalkingSpeed { get; private set; }
    [field: SerializeField] public float RunningSpeed { get; private set; }
    [field: SerializeField] public float SneakingSpeed { get; private set; }

    [field: SerializeField] public float StandingHeight { get; private set; }
    [field: SerializeField] public float StandingOffset { get; private set; }
    [field: SerializeField] public float SneakingHeight { get; private set; }
    [field: SerializeField] public float SneakingOffset { get; private set; }

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
}
