using UnityEngine;

public class PlayerFreeLookState : PlayerBaseState
{
    private readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLook Blend Tree");
    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.5f;
    private const float RunningHash = 1;
    private const float WalkingHash = 0.5f;
    public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputReader.CrouchEvent += SwitchToSneaking;
        stateMachine.StandUp();
        stateMachine.Animator.CrossFadeInFixedTime(FreeLookBlendTreeHash, CrossFadeDuration);
    }

    public override void Exit()
    {
        stateMachine.InputReader.CrouchEvent -= SwitchToSneaking;
    }

    public override void Tick(float deltaTime)
    {
        float speed, blendHash, baseNoise, emitIntensity;
        if (stateMachine.InputReader.IsSprint)
        {
            speed = stateMachine.RunningSpeed;
            blendHash = RunningHash;
            baseNoise = stateMachine.RunningBaseNoise;
            emitIntensity = stateMachine.RunningEmitIntensity;
        }
        else
        {
            speed = stateMachine.WalkingSpeed;
            blendHash = WalkingHash;
            baseNoise = stateMachine.WalkingBaseNoise;
            emitIntensity = stateMachine.WalkingEmitIntensity;
        }

        Vector3 movement = CalculateMovement();
        Move(movement * speed, deltaTime);

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.NoiseSource.ResetBaseNoiseLevel();
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }

        stateMachine.NoiseSource.SetBaseNoiseLevel(baseNoise);
        stateMachine.NoiseSource.EmitNoise(emitIntensity);

        stateMachine.Animator.SetFloat(FreeLookSpeedHash, blendHash, AnimatorDampTime, deltaTime);
        FaceMovementDirection(movement, deltaTime);
    }
    private Vector3 CalculateMovement()
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;

        //Ignore camera Y-axis movement
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

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