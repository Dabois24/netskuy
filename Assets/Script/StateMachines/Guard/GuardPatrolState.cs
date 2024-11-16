using UnityEngine;

public class GuardPatrolState : GuardBaseState
{
    private readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLook Blend Tree");
    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.5f;

    private int currentPatrolIndex;
    private bool isSentryMode;
    private float sentryRotationTimer;
    private bool isScanning;
    private float scanningTimer;

    public GuardPatrolState(GuardStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        if (stateMachine.PatrolPoint == null || stateMachine.PatrolPoint.Count <= 1)
        {
            isSentryMode = true;
        }
        else
        {
            isSentryMode = false;
            currentPatrolIndex = 0;
            MoveToNextPatrolPoint();
        }
        stateMachine.GuardSight.IsChasing = false;
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
        }

        if (!stateMachine.GuardSight.IsPlayerDetected)
        {
            if (isSentryMode)
            {
                PerformSentryRotation(deltaTime);
                stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            }
            else
            {
                if (!stateMachine.Agent.pathPending && stateMachine.Agent.remainingDistance <= stateMachine.Agent.stoppingDistance)
                {

                    if (!isScanning)
                    {
                        BeginScanning();
                    }
                    else
                    {
                        PerformSentryScan(deltaTime);
                    }

                    stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
                }
                else
                {
                    MoveToDestination(stateMachine.PatrolPoint[currentPatrolIndex].position, deltaTime);
                    stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0.5f, AnimatorDampTime, deltaTime);
                }
            }
        }
        else
        {
            FaceTarget(stateMachine.GuardSight.Player.position, deltaTime);
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
        }
    }

    public override void Exit()
    {
        stateMachine.Agent.ResetPath();
        stateMachine.Agent.velocity = Vector3.zero;
    }

    private void PerformSentryRotation(float deltaTime)
    {
        sentryRotationTimer += deltaTime;
        float angle = Mathf.PingPong(sentryRotationTimer * stateMachine.sentryRotateSpeed,
            stateMachine.sentryMaxAngle - stateMachine.sentryMinAngle) + stateMachine.sentryMinAngle;
        stateMachine.transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void BeginScanning()
    {
        isScanning = true;
        scanningTimer = 0f;
    }

    private void PerformSentryScan(float deltaTime)
    {
        if (stateMachine.GuardSight.IsPlayerDetected)
        {
            isScanning = false;
            return;
        }

        scanningTimer += deltaTime;

        if (scanningTimer < stateMachine.sentryIdleDuration)
        {
            // Guard idles at the destination
            return;
        }

        float scanProgress = (scanningTimer - stateMachine.sentryIdleDuration) / stateMachine.sentryScanDuration;
        if (scanProgress <= 0.5f)
        {
            // Scanning left
            float leftAngle = Mathf.Lerp(0f, stateMachine.sentryMinAngle, scanProgress * 2f);
            stateMachine.transform.rotation = Quaternion.Euler(0f, leftAngle, 0f);
        }
        else if (scanProgress <= 1f)
        {
            // Scanning right
            float rightAngle = Mathf.Lerp(stateMachine.sentryMinAngle, stateMachine.sentryMaxAngle, (scanProgress - 0.5f) * 2f);
            stateMachine.transform.rotation = Quaternion.Euler(0f, rightAngle, 0f);
        }
        else
        {
            // Scanning complete, move to the next patrol point
            isScanning = false;
            currentPatrolIndex = (currentPatrolIndex + 1) % stateMachine.PatrolPoint.Count;
            MoveToNextPatrolPoint();
        }
    }

    private void MoveToNextPatrolPoint()
    {
        if (stateMachine.PatrolPoint == null || stateMachine.PatrolPoint.Count == 0)
        {
            return;
        }

        Vector3 nextPoint = stateMachine.PatrolPoint[currentPatrolIndex].position;
        stateMachine.Agent.SetDestination(nextPoint);
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
    }
}
