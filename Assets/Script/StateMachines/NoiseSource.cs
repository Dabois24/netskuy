using UnityEngine;
using System.Collections;

public class NoiseSource : MonoBehaviour
{
    [Header("Noise Settings")]
    [SerializeField] private float baseNoiseLevel = 0f; // Default noise level
    [SerializeField] private float maxNoiseRange = 15f; // Max range where noise can be heard
    [SerializeField] private float noiseDecayRate = 1f; // Rate at which noise diminishes over time

    public float NoiseLevel { get; private set; }

    private float originalBaseNoiseLevel; // To store the initial base noise level
    private Coroutine temporaryNoiseCoroutine;

    private void Awake()
    {
        originalBaseNoiseLevel = baseNoiseLevel;
        NoiseLevel = baseNoiseLevel;
    }

    public void EmitNoise(float intensity)
    {
        NoiseLevel = Mathf.Clamp(baseNoiseLevel + intensity, 0f, maxNoiseRange);
    }

    private void Update()
    {
        // Decay noise level over time
        if (NoiseLevel > baseNoiseLevel)
        {
            NoiseLevel = Mathf.Max(baseNoiseLevel, NoiseLevel - noiseDecayRate * Time.deltaTime);
        }
    }

    public void SetTemporaryBaseNoiseLevel(float newBaseLevel, float duration)
    {
        if (temporaryNoiseCoroutine != null)
        {
            StopCoroutine(temporaryNoiseCoroutine);
        }

        temporaryNoiseCoroutine = StartCoroutine(TemporaryBaseNoiseCoroutine(newBaseLevel, duration));
    }

    public void ResetBaseNoiseLevel()
    {
        if (temporaryNoiseCoroutine != null)
        {
            StopCoroutine(temporaryNoiseCoroutine);
        }

        baseNoiseLevel = originalBaseNoiseLevel;
        NoiseLevel = Mathf.Max(NoiseLevel, baseNoiseLevel); 
    }

    private IEnumerator TemporaryBaseNoiseCoroutine(float newBaseLevel, float duration)
    {
        baseNoiseLevel = newBaseLevel;
        NoiseLevel = Mathf.Max(NoiseLevel, baseNoiseLevel);

        yield return new WaitForSeconds(duration);

        ResetBaseNoiseLevel();
    }

    public void SetBaseNoiseLevel(float newBaseLevel)
    {
        baseNoiseLevel = newBaseLevel;
        NoiseLevel = Mathf.Max(NoiseLevel, baseNoiseLevel); 
    }

    private void OnDrawGizmos()
    {
        // Visualize the max noise range
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, maxNoiseRange);

        // Visualize the current noise level
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, NoiseLevel);
    }
}
