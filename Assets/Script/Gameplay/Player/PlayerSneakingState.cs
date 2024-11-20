using UnityEngine;

public class PlayerSneakingState : PlayerBaseState
{
    private readonly int SneakingBlendTreeHash = Animator.StringToHash("Sneaking Blend Tree");
    private readonly int SneakingSpeedHash = Animator.StringToHash("SneakingSpeed");
    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.5f;
    private float volumeMultiplier = 0.5f;
    public PlayerSneakingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputReader.CrouchEvent += SwitchToFreeLook;
        stateMachine.Crouch();
        stateMachine.RunSfx.enabled = false;
        stateMachine.WalkSfx.volume = stateMachine.WalkSfx.volume * volumeMultiplier;
        stateMachine.Animator.CrossFadeInFixedTime(SneakingBlendTreeHash, CrossFadeDuration);
    }

    public override void Exit()
    {
        stateMachine.InputReader.CrouchEvent -= SwitchToFreeLook;
        stateMachine.WalkSfx.volume = stateMachine.WalkSfx.volume / volumeMultiplier;
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = CalculateMovement();
        Move(movement * stateMachine.SneakingSpeed, deltaTime);
        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.WalkSfx.enabled = false;
            stateMachine.Animator.SetFloat(SneakingSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }

        stateMachine.WalkSfx.enabled = true;
        stateMachine.NoiseSource.SetMaxNoiseRange(stateMachine.SneakingMaxNoise);
        stateMachine.NoiseSource.EmitNoise(stateMachine.SneakingEmitIntensity);

        stateMachine.Animator.SetFloat(SneakingSpeedHash, 1, AnimatorDampTime, deltaTime);
        FaceMovementDirection(movement, deltaTime);
    }

    private Vector3 CalculateMovement()
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;

        //Ignore camera Y-axis movement
        forward.y = 0f;
        right.y = 0f;

        return forward * stateMachine.InputReader.MovementValue.y +
            right * stateMachine.InputReader.MovementValue.x;
    }
    private void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * stateMachine.RotationDamping);
    }
}