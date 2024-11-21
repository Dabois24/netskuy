using UnityEngine;

public class GuardChaseState : GuardBaseState
{
    private readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLook Blend Tree");
    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.5f;

    public GuardChaseState(GuardStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.GuardSight.IsChasing = true;
        stateMachine.GuardSight.ResetTimers();
        stateMachine.Animator.CrossFadeInFixedTime(FreeLookBlendTreeHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.GuardSight.UpdateSight();

        if (stateMachine.GuardSight.LosePlayerForSeconds(stateMachine.GuardSight.lostSightTime))
        {
            Debug.Log("Lost sight of player. Transitioning to patrol state.");
            stateMachine.SwitchState(new GuardPatrolState(stateMachine));
            return;
        }
        else
        {
            stateMachine.DetectionIndicator.UpdateDetectionLoseBar(stateMachine.GuardSight.lostSightTimer, stateMachine.GuardSight.lostSightTime);
        }

        // Chase the player
        MoveToDestination(stateMachine.GuardSight.Player.position, deltaTime);

        // Check for capture conditions
        if (Vector3.Distance(stateMachine.transform.position, stateMachine.GuardSight.Player.position) <= stateMachine.CaptureDistance && !stateMachine.GuardSight.IsPlayerObstructed)
        {
            if (stateMachine.GuardSight.Player.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine player))
            {
                if (player.IsTargetable)
                {
                    stateMachine.GuardSight.StartCaptureCountdown(() =>
                    {
                        Debug.Log("Player busted.");
                        player.Busted();
                    });
                }
                else
                {
                    stateMachine.SwitchState(new GuardVictoryState(stateMachine));
                    return;
                }
            }
        }
        else
        {
            stateMachine.GuardSight.StopCaptureCountdown();
        }

        stateMachine.Animator.SetFloat(FreeLookSpeedHash, 1, AnimatorDampTime, deltaTime);
    }

    public override void Exit()
    {
        stateMachine.AudioSource.enabled = false;
        stateMachine.GuardSight.IsChasing = false;
        stateMachine.Agent.ResetPath();
        stateMachine.GuardSight.ResetTimers();
        stateMachine.GuardSight.StopCaptureCountdown();
    }

    private void MoveToDestination(Vector3 destination, float deltaTime)
    {
        if (stateMachine.Agent.isOnNavMesh)
        {
            stateMachine.Agent.destination = destination;

            Move(stateMachine.Agent.desiredVelocity.normalized * stateMachine.ChaseSpeed, deltaTime);
        }

        stateMachine.Agent.velocity = stateMachine.Controller.velocity;
        FaceTarget(destination, deltaTime);
        stateMachine.AudioSource.enabled = true;
    }
}
