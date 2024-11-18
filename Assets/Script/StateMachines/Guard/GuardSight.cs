using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GuardSight : MonoBehaviour
{
    [Header("State")]
    public bool IsChasing = false;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 10f; // Maximum range
    [SerializeField] private float fieldOfViewAngle = 90f; // Cone angle (centered forward)
    [SerializeField] private List<Transform> raycastOrigins = new List<Transform>(); // Raycast origins
    [SerializeField] private LayerMask detectionLayer;   // Player layer
    [SerializeField] private LayerMask obstructionLayer; // Obstacle layer

    [field: SerializeField] public float detectionTime { get; private set; } = 5f;
    [field: SerializeField] public float lostSightTime { get; private set; } = 10f;
    [field: SerializeField] public float captureCountdownTime { get; private set; } = 6f;

    public bool IsPlayerDetected { get; private set; }
    public bool IsPlayerObstructed { get; private set; }
    public Transform Player { get; private set; }

    private Coroutine captureCountdownCoroutine;
    private float detectionTimer;
    private float lostSightTimer;

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

        IsPlayerDetected = false;
        IsPlayerObstructed = false;

        Vector3 toPlayer = Player.position - transform.position;

        // Player must be within detection range and FOV
        if (!(toPlayer.magnitude <= detectionRange && IsInFieldOfView(toPlayer))) return;

        foreach (Transform origin in raycastOrigins)
        {
            Vector3 direction = (Player.position - origin.position).normalized;

            if (Physics.Raycast(origin.position, direction, out RaycastHit hit, detectionRange, detectionLayer | obstructionLayer))
            {
                if (((1 << hit.collider.gameObject.layer) & detectionLayer) != 0)
                {
                    IsPlayerDetected = true;
                    Debug.DrawLine(origin.position, hit.point, Color.green); // Player detected
                }
                else
                {
                    IsPlayerObstructed = true;
                    Debug.DrawLine(origin.position, hit.point, Color.red); // Obstruction detected
                }
            }
            else
            {
                Debug.DrawLine(origin.position, origin.position + direction * detectionRange, Color.yellow); // Nothing hit
            }
        }
    }

    private bool IsInFieldOfView(Vector3 toPlayer)
    {
        Vector3 forward = transform.forward;
        float angleToPlayer = Vector3.Angle(forward, toPlayer.normalized);
        return angleToPlayer <= fieldOfViewAngle / 2f;
    }

    public bool DetectPlayerForSeconds(float seconds)
    {
        if (IsPlayerDetected)
        {
            detectionTimer += Time.deltaTime;
            // Debug.Log($"Detection Timer: {detectionTimer}");
            return detectionTimer >= seconds;
        }
        else
        {
            detectionTimer = 0;
            return false;
        }
    }

    public bool LosePlayerForSeconds(float seconds)
    {
        if (IsPlayerObstructed)
        {
            lostSightTimer += Time.deltaTime;
            return lostSightTimer >= seconds;
        }
        else
        {
            lostSightTimer = 0;
            return false;
        }
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

        Gizmos.color = Color.yellow;

        foreach (var origin in raycastOrigins)
        {
            // // Draw the detection range
            // Gizmos.DrawWireSphere(origin.position, detectionRange);

            // Draw the field of view
            Vector3 forward = origin.forward;
            Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfViewAngle / 2, 0) * forward * detectionRange;
            Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle / 2, 0) * forward * detectionRange;

            Gizmos.DrawLine(origin.position, origin.position + leftBoundary);
            Gizmos.DrawLine(origin.position, origin.position + rightBoundary);
        }
    }
}
