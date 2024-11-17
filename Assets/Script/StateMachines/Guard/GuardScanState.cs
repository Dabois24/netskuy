using UnityEngine;

public class GuardScanState : GuardBaseState
{
    private readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLook Blend Tree");
    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.5f;

    private bool isInterrupted;
    private float scanningTimer;
    private Quaternion initialRotation;
    private Quaternion interruptedRotation;

    public GuardScanState(GuardStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        initialRotation = stateMachine.transform.rotation;
        scanningTimer = 0f;
        isInterrupted = false;

        stateMachine.Animator.CrossFadeInFixedTime(FreeLookBlendTreeHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.GuardSight.UpdateSight();

        if (HandlePlayerDetection(deltaTime))
            return;

        if (HandleInterruption(deltaTime))
            return;

        PerformScanning(deltaTime);
    }

    public override void Exit()
    {
        stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, 0f);
    }

    private bool HandlePlayerDetection(float deltaTime)
    {
        if (stateMachine.GuardSight.DetectPlayerForSeconds(stateMachine.GuardSight.detectionTime))
        {
            Debug.Log("Player detected. Transitioning to chase state.");
            stateMachine.SwitchState(new GuardChaseState(stateMachine));
            return true;
        }

        if (stateMachine.GuardSight.IsPlayerDetected)
        {
            isInterrupted = true;
            interruptedRotation = stateMachine.transform.rotation;
            FaceTargetDirect(stateMachine.GuardSight.Player.position, deltaTime);
            return true;
        }

        return false;
    }

    private bool HandleInterruption(float deltaTime)
    {
        if (!isInterrupted)
            return false;

        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            interruptedRotation,
            deltaTime * stateMachine.RotationDamping);

        if (Quaternion.Angle(stateMachine.transform.rotation, interruptedRotation) < 0.1f)
        {
            isInterrupted = false;
        }

        return true;
    }

    private void PerformScanning(float deltaTime)
    {
        scanningTimer += deltaTime;

        if (scanningTimer < stateMachine.sentryIdleDuration)
        {
            // Initial idle duration
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }

        float scanProgress = (scanningTimer - stateMachine.sentryIdleDuration) / stateMachine.sentryScanDuration;

        if (scanProgress <= 0.333f)
        {
            // Scan left
            PerformLeftScan(scanProgress * 3f);
        }
        else if (scanProgress <= 0.666f)
        {
            // Scan right from the leftmost position
            PerformRightScan((scanProgress - 0.333f) * 3f);
        }
        else if (scanProgress <= 1f)
        {
            // Return to initial rotation from the rightmost position
            PerformReturnScan((scanProgress - 0.666f) * 3f);
        }
        else
        {
            // Scanning complete, return to patrol
            stateMachine.currentPatrolIndex = (stateMachine.currentPatrolIndex + 1) % stateMachine.PatrolPoint.Count;
            stateMachine.SwitchState(new GuardPatrolState(stateMachine));
        }
    }

    private void PerformLeftScan(float progress)
    {
        float leftAngle = Mathf.Lerp(0f, stateMachine.scanMinAngle, progress);
        stateMachine.transform.rotation = Quaternion.Euler(
            initialRotation.eulerAngles.x,
            initialRotation.eulerAngles.y + leftAngle,
            initialRotation.eulerAngles.z);

        stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, Time.deltaTime);
    }

    private void PerformRightScan(float progress)
    {
        float rightAngle = Mathf.Lerp(stateMachine.scanMinAngle, stateMachine.scanMaxAngle, progress);
        stateMachine.transform.rotation = Quaternion.Euler(
            initialRotation.eulerAngles.x,
            initialRotation.eulerAngles.y + rightAngle,
            initialRotation.eulerAngles.z);

        stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, Time.deltaTime);
    }

    private void PerformReturnScan(float progress)
    {
        float returnAngle = Mathf.Lerp(stateMachine.scanMaxAngle, 0f, progress);
        stateMachine.transform.rotation = Quaternion.Euler(
            initialRotation.eulerAngles.x,
            initialRotation.eulerAngles.y + returnAngle,
            initialRotation.eulerAngles.z);

        stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, Time.deltaTime);
    }
}
