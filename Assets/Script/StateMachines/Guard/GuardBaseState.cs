using UnityEngine;

public abstract class GuardBaseState : State
{
    protected GuardStateMachine stateMachine;

    protected GuardBaseState(GuardStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void Move(float deltaTime)
    {
        Move(Vector3.zero, deltaTime);
    }

    protected void Move(Vector3 motion, float deltaTime)
    {
        stateMachine.Controller.Move((motion + stateMachine.ForceReceiver.Movement) * deltaTime);
    }

    protected void FaceTarget(Vector3 target, float deltaTime)
    {
        Vector3 lookPos = target - stateMachine.transform.position;
        lookPos.y = 0f;

        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(lookPos),
            deltaTime * stateMachine.RotationDamping);
    }
}
