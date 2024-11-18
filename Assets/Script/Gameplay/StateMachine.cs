using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State currentState;

    public void SwitchState(State newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();

        Debug.Log(currentState?.ToString());
    }

    private void Update()
    {
        currentState?.Tick(Time.deltaTime);
    }
}
