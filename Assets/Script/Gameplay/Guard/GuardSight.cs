using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GuardSight : MonoBehaviour
{
    [Header("State")]
    public bool IsChasing;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float fieldOfViewAngle = 90f;
    [SerializeField] private List<Transform> raycastOrigins = new List<Transform>();
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private LayerMask obstructionLayer;

    [field: SerializeField] public float detectionTime { get; private set; } = 5f;
    [field: SerializeField] public float lostSightTime { get; private set; } = 10f;
    [field: SerializeField] public float captureCountdownTime { get; private set; } = 6f;

    public float detectionTimer { get; private set; }
    public float lostSightTimer { get; private set; }
    public bool IsPlayerDetected { get; private set; }
    public bool IsPlayerObstructed { get; private set; }
    public Transform Player { get; private set; }

    private Coroutine captureCountdownCoroutine;

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (Player == null)
        {
            Debug.LogError("Player not found. Make sure the player has the 'Player' tag.");
        }

        if (raycastOrigins.Count == 0)
        {
            raycastOrigins.Add(transform); // Fallback to self
        }
    }

    public void UpdateSight()
    {
        if (Player == null) return;

        ResetDetectionFlags();

        if (IsChasing)
        {
            CheckChaseDetection();
        }
        else
        {
            CheckNormalDetection();
        }
    }

    private void ResetDetectionFlags()
    {
        IsPlayerDetected = false;
        IsPlayerObstructed = false;
    }

    private void CheckChaseDetection()
    {
        foreach (Transform origin in raycastOrigins)
        {
            PerformRaycast(origin, ignoreYAxis: true);
        }

        foreach (Transform origin in raycastOrigins)
        {
            PerformRaycast(origin, ignoreYAxis: false);
        }
    }

    private void CheckNormalDetection()
    {
        Vector3 toPlayer = Player.position - transform.position;
        if (!(toPlayer.magnitude <= detectionRange && IsInFieldOfView(toPlayer))) return;

        foreach (Transform origin in raycastOrigins)
        {
            PerformRaycast(origin, ignoreYAxis: false);
        }
    }

    private void PerformRaycast(Transform origin, bool ignoreYAxis)
    {
        Vector3 direction = Player.position - origin.position;
        if (!ignoreYAxis) direction.y = 0;
        direction.Normalize();

        if (Physics.Raycast(origin.position, direction, out RaycastHit hit, detectionRange, detectionLayer | obstructionLayer))
        {
            if (((1 << hit.collider.gameObject.layer) & detectionLayer) != 0)
            {
                IsPlayerDetected = true;
                // Debug.Log($"Player detected by {origin.name} at {hit.point}");
                Debug.DrawLine(origin.position, hit.point, Color.green);
            }
            else
            {
                IsPlayerObstructed = true;
                // Debug.Log($"Obstruction detected by {origin.name} at {hit.point}");
                Debug.DrawLine(origin.position, hit.point, Color.red);
            }
        }
        else
        {
            Debug.DrawLine(origin.position, origin.position + direction * detectionRange, Color.yellow);
        }
    }

    private bool IsInFieldOfView(Vector3 toPlayer)
    {
        Vector3 forward = transform.forward;
        float angleToPlayer = Vector3.Angle(forward, toPlayer.normalized);
        return angleToPlayer <= fieldOfViewAngle / 2f;
    }

    private bool CheckTimer(Action<float> updateTimer, Func<float> getTimer, float threshold, bool condition)
    {
        if (condition)
        {
            updateTimer(Mathf.Min(getTimer() + Time.deltaTime, threshold));
            return getTimer() >= threshold;
        }
        else
        {
            updateTimer(Mathf.Max(getTimer() - Time.deltaTime, 0));
            return false;
        }
    }

    public bool DetectPlayerForSeconds(float seconds)
    {
        return CheckTimer(
            value => detectionTimer = value,
            () => detectionTimer,
            seconds,
            IsPlayerDetected
        );
    }

    public bool LosePlayerForSeconds(float seconds)
    {
        return CheckTimer(
            value => lostSightTimer = value,
            () => lostSightTimer,
            seconds,
            IsPlayerObstructed && !IsPlayerDetected
        );
    }

    public void ResetTimers()
    {
        detectionTimer = 0;
        lostSightTimer = 0;
    }

    public void StartCaptureCountdown(Action onCaptureComplete)
    {
        if (captureCountdownCoroutine == null)
        {
            captureCountdownCoroutine = StartCoroutine(CaptureCountdown(onCaptureComplete));
        }
    }

    public void StopCaptureCountdown()
    {
        if (captureCountdownCoroutine != null)
        {
            StopCoroutine(captureCountdownCoroutine);
            captureCountdownCoroutine = null;
        }
    }

    private IEnumerator CaptureCountdown(Action onCaptureComplete)
    {
        float timer = captureCountdownTime;

        while (timer > 0)
        {
            Debug.Log($"Capture in {timer:F1} seconds...");
            timer -= Time.deltaTime;
            yield return null;
        }

        onCaptureComplete?.Invoke();
        captureCountdownCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        if (raycastOrigins == null || raycastOrigins.Count == 0) return;

        Gizmos.color = IsChasing ? Color.red : Color.yellow;

        foreach (var origin in raycastOrigins)
        {
            Vector3 forward = origin.forward;
            Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfViewAngle / 2, 0) * forward * detectionRange;
            Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle / 2, 0) * forward * detectionRange;

            Gizmos.DrawLine(origin.position, origin.position + leftBoundary);
            Gizmos.DrawLine(origin.position, origin.position + rightBoundary);
        }
    }
}
