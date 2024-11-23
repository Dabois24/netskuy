using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private PlayerStateMachine player;
    [SerializeField] private WaypointMarker waypointMarker;
    [SerializeField] private float countdownDuration = 3f;
    private bool isTriggered;
    private float countdownTimer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStateMachine>();
        countdownTimer = countdownDuration;
        isTriggered = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isTriggered) return;
        if (other.CompareTag("Player"))
        {
            countdownTimer -= Time.deltaTime;

            waypointMarker.UpdateBar(countdownDuration - countdownTimer, countdownDuration);

            if (countdownTimer <= 0)
            {
                isTriggered = true;
                player.Win();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            countdownTimer = countdownDuration;

            waypointMarker.UpdateBar(0, countdownDuration);
        }
    }
}
