using UnityEngine;

public class GuardInvestigateState : GuardBaseState
{
    private readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLook Blend Tree");
    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.5f;

    private Vector3 noisePosition;

    public GuardInvestigateState(GuardStateMachine stateMachine, Vector3 noisePosition) : base(stateMachine)
    {
        this.noisePosition = noisePosition;
    }

    public override void Enter()
    {
        Debug.Log($"Investigating noise at {noisePosition}");
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

        if (stateMachine.GuardSight.IsPlayerDetected)
        {
            StopMove(deltaTime);
            FaceTargetDirect(stateMachine.GuardSight.Player.position, deltaTime);
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }

        // Rotate towards noise source
        Vector3 direction = (noisePosition - stateMachine.transform.position).normalized;
        FaceTarget(direction, deltaTime);

        // If close enough, switch to scan
        if (Vector3.Distance(stateMachine.transform.position, noisePosition) < stateMachine.InvestigateToScanDistance)
        {
            Debug.Log("Finished investigating noise. Returning to patrol.");
            stateMachine.SwitchState(new GuardScanState(stateMachine));
            return;
        }

        MoveToDestination(noisePosition, deltaTime);
        stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0.5f, AnimatorDampTime, deltaTime);
    }

    public override void Exit()
    {
        stateMachine.GuardHear.ResetHearing();
    }
    private void MoveToDestination(Vector3 destination, float deltaTime)
    {
        if (stateMachine.Agent.isOnNavMesh)
        {
            stateMachine.Agent.destination = destination;
            Move(stateMachine.Agent.desiredVelocity.normalized * stateMachine.InvestigateSpeed, deltaTime);
        }

        stateMachine.Agent.velocity = stateMachine.Controller.velocity;
        FaceTarget(destination, deltaTime);
    }
}
