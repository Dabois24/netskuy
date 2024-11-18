using UnityEngine;

public class GuardHear : MonoBehaviour
{
    [Header("Hearing Settings")]
    [SerializeField] private float detectionTime = 2f;

    private float detectionTimer;
    private bool isNoiseDetected;
    public Vector3 LastHeardPosition { get; private set; }
    private Transform detectedNoiseSource;

    public bool IsNoiseDetected => isNoiseDetected;
    public Transform DetectedNoiseSource => detectedNoiseSource;

    public void ResetDetection()
    {
        isNoiseDetected = false;
        detectedNoiseSource = null;
        detectionTimer = 0;
    }

    public void ProcessNoise(Transform noiseSource, float noiseLevel, bool isImpulse)
    {
        // Handle impulse noise
        if (isImpulse)
        {
            DetectNoise(noiseSource);
            return;
        }

        // Gradual detection based on timer
        detectionTimer += Time.deltaTime;
        if (detectionTimer >= detectionTime)
        {
            DetectNoise(noiseSource);
        }
    }

    private void DetectNoise(Transform noiseSource)
    {
        Debug.Log("Noise Detected.");
        isNoiseDetected = true;
        detectedNoiseSource = noiseSource;
        LastHeardPosition = noiseSource.position;
        detectionTimer = 0; // Reset detection timer
    }

    private void OnDrawGizmos()
    {
        // Draw the guard's position with a sphere
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f); // Small cyan sphere for the guard
    }
}
