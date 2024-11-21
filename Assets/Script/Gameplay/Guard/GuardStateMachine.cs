using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ForceReceiver), typeof(GuardSight), typeof(GuardHear))]
public class GuardStateMachine : StateMachine
{
    [field: Header("Component")]
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public GuardSight GuardSight { get; private set; }
    [field: SerializeField] public GuardHear GuardHear { get; private set; }
    [field: SerializeField] public DetectionIndicator DetectionIndicator { get; private set; }
    [field: SerializeField] public AudioSource AudioSource { get; private set; }

    [field: Header("Patrol Setting")]
    [field: SerializeField] public float PatrolSpeed { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }
    [field: SerializeField] public float scanMinAngle { get; private set; } = -45f;
    [field: SerializeField] public float scanMaxAngle { get; private set; } = 45f;
    [field: SerializeField] public float scanRotateSpeed { get; private set; } = 50f;
    [field: SerializeField] public float sentryScanDuration { get; private set; } = 3f;
    [field: SerializeField] public float sentryIdleDuration { get; private set; } = 2f;
    [field: SerializeField] public List<Transform> PatrolPoint { get; private set; }
    public int currentPatrolIndex;

    [field: Header("Chasing Setting")]
    [field: SerializeField] public float ChaseSpeed { get; private set; }
    [field: SerializeField] public float CaptureDistance { get; private set; } = 1f; // 1 meter

    [field: Header("Investigate Setting")]
    [field: SerializeField] public float InvestigateSpeed { get; private set; }
    [field: SerializeField] public float InvestigateToScanDistance { get; private set; } = 5f; // 1 meter

    [field: Header("Victory Animations")]
    [field: SerializeField] public string[] VictoryAnimationNames { get; private set; } = {"Victory 0", "Victory 1", "Victory 2", "Victory 3"};

    private void Start()
    {
        Agent.updatePosition = false;
        Agent.updateRotation = false;
        SwitchState(new GuardPatrolState(this));
    }

    public void CreatePatrolPoint()
    {
        if (PatrolPoint == null)
        {
            PatrolPoint = new List<Transform>();
        }

        GameObject newPatrolPoint = new GameObject($"Patrol Point {PatrolPoint.Count + 1}");
        newPatrolPoint.transform.position = transform.position;

        PatrolPoint.Add(newPatrolPoint.transform);
    }

}
