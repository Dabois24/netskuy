using UnityEngine;

public abstract class PlayerBaseState : State{
protected PlayerStateMachine stateMachine;

    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void Move(float deltaTime)
    {
        Move(Vector3.zero, deltaTime);
    }

    protected void Move(Vector3 motion, float deltaTime)
    {
        Debug.Log($"motion: {motion} + {stateMachine.ForceReceiver.Movement}; ");
        stateMachine.Controller.Move((motion + stateMachine.ForceReceiver.Movement) * deltaTime);
    }
    public void SwitchToSneaking()
    {
        stateMachine.SwitchState(new PlayerSneakingState(stateMachine));
    }
    public void SwitchToFreeLook(){
        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
    }
}