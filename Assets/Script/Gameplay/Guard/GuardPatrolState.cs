using UnityEngine;

public class GuardPatrolState : GuardBaseState
{
    private readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLook Blend Tree");
    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.5f;

    public GuardPatrolState(GuardStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        if (stateMachine.PatrolPoint == null || stateMachine.PatrolPoint.Count == 0)
        {
            stateMachine.CreatePatrolPoint();
            stateMachine.SwitchState(new GuardScanState(stateMachine));
        }

        stateMachine.GuardSight.ResetTimers();
        stateMachine.Animator.CrossFadeInFixedTime(FreeLookBlendTreeHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.GuardSight.UpdateSight();

        if (stateMachine.GuardSight.DetectPlayerForSeconds(stateMachine.GuardSight.detectionTime))
        {
            Debug.Log("Player detected. Transitioning to chase state.");
            stateMachine.SwitchState(new GuardChaseState(stateMachine));
            return;
        }
        else
        {
            stateMachine.DetectionIndicator.UpdateDetectionBar(stateMachine.GuardSight.detectionTimer, stateMachine.GuardSight.detectionTime);
        }

        if (stateMachine.GuardSight.IsPlayerDetected)
        {
            StopMove(deltaTime);
            FaceTargetDirect(stateMachine.GuardSight.Player.position, deltaTime);
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }

        if (stateMachine.GuardHear.IsNoiseDetected)
        {
            Debug.Log("Noise detected! Switching to Investigate State.");
            stateMachine.SwitchState(new GuardInvestigateState(stateMachine, stateMachine.GuardHear.LastHeardPosition));
            return;
        }

        MoveToDestination(stateMachine.PatrolPoint[stateMachine.currentPatrolIndex].position, deltaTime);
        stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0.5f, AnimatorDampTime, deltaTime);

        if (!stateMachine.Agent.pathPending && stateMachine.Agent.remainingDistance <= stateMachine.Agent.stoppingDistance)
        {
            stateMachine.SwitchState(new GuardScanState(stateMachine));
            return;
        }

    }

    public override void Exit()
    {
        stateMachine.AudioSource.enabled = false;
        stateMachine.Agent.ResetPath();
        stateMachine.Agent.velocity = Vector3.zero;
    }

    private void MoveToDestination(Vector3 destination, float deltaTime)
    {
        if (stateMachine.Agent.isOnNavMesh)
        {
            stateMachine.Agent.destination = destination;
            Move(stateMachine.Agent.desiredVelocity.normalized * stateMachine.PatrolSpeed, deltaTime);
        }

        stateMachine.Agent.velocity = stateMachine.Controller.velocity;
        FaceTarget(destination, deltaTime);
        stateMachine.AudioSource.enabled = true;
    }
}
