using UnityEngine;

public class PlayerVictoryState : PlayerBaseState
{
    private const float CrossFadeDuration = 0.5f;
    private float timer;
    public PlayerVictoryState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        timer = stateMachine.VictoryScreenWaitingTime;
        string animationName = stateMachine.VictoryAnimationNames[Random.Range(0, stateMachine.VictoryAnimationNames.Length)];
        stateMachine.Animator.CrossFadeInFixedTime(animationName, CrossFadeDuration);
    }

    public override void Exit()
    {

    }

    public override void Tick(float deltaTime)
    {
        if (timer < 0) return;
        timer -= deltaTime;
        if (timer <= 0)
        {
            stateMachine.OnVictoryWaitCompleted();
        }
    }
}