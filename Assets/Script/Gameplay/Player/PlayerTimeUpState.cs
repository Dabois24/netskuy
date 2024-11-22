using UnityEngine;

public class PlayerTimeUpState : PlayerBaseState
{
    private const float CrossFadeDuration = 0.5f;
    public PlayerTimeUpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        string animationName = stateMachine.TimeUpAnimationNames[Random.Range(0, stateMachine.VictoryAnimationNames.Length)];
        stateMachine.Animator.CrossFadeInFixedTime(animationName, CrossFadeDuration);
    }

    public override void Exit()
    {

    }

    public override void Tick(float deltaTime)
    {

    }
}