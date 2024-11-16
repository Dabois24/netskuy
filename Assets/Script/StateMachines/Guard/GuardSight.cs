using UnityEngine;
using System;
using System.Collections;

public class GuardSight : MonoBehaviour
{
    [Header("State")]
    public bool IsChasing = false;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private LayerMask detectionLayer;   // Layer for the player
    [SerializeField] private LayerMask obstructionLayer; // Layer for obstacles

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
    }

    public void UpdateSight()
    {
        // if (Player == null) return;

        Transform origin = raycastOrigin != null ? raycastOrigin : transform;

        Vector3 facingDirection = Player != null && (IsChasing || IsPlayerDetected) ? (Player.position - transform.position).normalized : transform.forward;

        if (Physics.Raycast(origin.position, facingDirection, out RaycastHit hit, detectionRange, detectionLayer | obstructionLayer))
        {
            if (((1 << hit.collider.gameObject.layer) & detectionLayer) != 0)
            {
                IsPlayerDetected = true;
                IsPlayerObstructed = false;
                Debug.DrawLine(origin.position, hit.point, Color.green);
                return;
            }
            else
            {
                IsPlayerObstructed = true;
                Debug.DrawLine(origin.position, hit.point, Color.red);
            }
        }
        else
        {
            IsPlayerDetected = false;
            IsPlayerObstructed = true;
            Debug.DrawLine(origin.position, origin.position + facingDirection * detectionRange, Color.red);
        }
    }

    public bool DetectPlayerForSeconds(float seconds)
    {
        if (IsPlayerDetected)
        {
            detectionTimer += Time.deltaTime;

            Debug.Log($"detection Timer : {detectionTimer}");

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
}
