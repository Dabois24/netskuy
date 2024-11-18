using UnityEngine;

public class GuardHear : MonoBehaviour
{
    [Header("Hearing Settings")]
    [SerializeField] private float hearingRange = 15f; // Max distance for noise detection
    [SerializeField] private float noiseDetectionThreshold = 5f; // Minimum noise level to respond
    [SerializeField] private LayerMask noiseSourceLayer; // Layer for noise sources

    public bool HasHeardNoise { get; private set; }
    public Vector3 LastHeardPosition { get; private set; }

    private void Awake()
    {
        HasHeardNoise = false;
        LastHeardPosition = Vector3.zero;
    }

    public void ListenForNoise()
    {
        Collider[] noiseSources = Physics.OverlapSphere(transform.position, hearingRange, noiseSourceLayer);

        foreach (var source in noiseSources)
        {
            NoiseSource noise = source.GetComponent<NoiseSource>();
            if (noise != null && noise.NoiseLevel >= noiseDetectionThreshold)
            {
                Debug.Log($"Noise detected from {noise.transform.position}");
                HasHeardNoise = true;
                LastHeardPosition = noise.transform.position;
                return;
            }
        }

        // Reset if no noise detected
        HasHeardNoise = false;
    }

    public void ResetHearing()
    {
        HasHeardNoise = false;
        LastHeardPosition = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }

}
