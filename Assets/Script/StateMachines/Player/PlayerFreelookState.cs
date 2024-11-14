using Mono.Cecil;
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
        float speed, blendHash;
        if (stateMachine.InputReader.IsSprint)
        {
            speed = stateMachine.RunningSpeed;
            blendHash = RunningHash;
        }
        else
        {
            speed = stateMachine.WalkingSpeed;
            blendHash = WalkingHash;
        }

        Vector3 movement = CalculateMovement();
        Move(movement * speed, deltaTime);

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }
        stateMachine.Animator.SetFloat(FreeLookSpeedHash, blendHash, AnimatorDampTime, deltaTime);
        stateMachine.transform.rotation = Quaternion.LookRotation(movement);
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


}