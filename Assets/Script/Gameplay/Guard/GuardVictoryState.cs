using UnityEngine;

public class GuardVictoryState : GuardBaseState
{
    private const float CrossFadeDuration = 0.5f;
    public GuardVictoryState(GuardStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        string animationName = stateMachine.VictoryAnimationNames[Random.Range(0, stateMachine.VictoryAnimationNames.Length)];
        stateMachine.Animator.CrossFadeInFixedTime(animationName, CrossFadeDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick(float deltaTime)
    {
    }
}