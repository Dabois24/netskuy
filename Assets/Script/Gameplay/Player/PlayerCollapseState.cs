using UnityEngine;

public class PlayerCollapseState : PlayerBaseState
{
    private readonly int CollapsedHash = Animator.StringToHash("Collapsed");
    private const float CrossFadeDuration = 0.5f;
    public PlayerCollapseState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(CollapsedHash, CrossFadeDuration);
    }

    public override void Exit()
    {

    }

    public override void Tick(float deltaTime)
    {
        AnimatorStateInfo stateInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Collapsed") && stateInfo.normalizedTime >= 1f)
        {
            stateMachine.OnCollapsedAnimationEnd();
        }
    }
}