using UnityEngine;

public class NoiseSource : MonoBehaviour
{
    [Header("Noise Settings")]
    [SerializeField] private float maxNoiseRange = 10f;
    [SerializeField] private float noiseDecayRate = 2f;
    [SerializeField] private float noiseDecayDelay = 1f;

    private float currentDecayDelay;
    private float currentNoiseRange;
    private bool wasImpulseNoise;

    public float CurrentNoiseRange => currentNoiseRange;

    public bool WasImpulseNoise
    {
        get
        {
            bool value = wasImpulseNoise;
            wasImpulseNoise = false; // Reset after check
            return value;
        }
    }

    private bool isActive = false;

    private void Update()
    {
        if (!isActive) return;

        currentDecayDelay -= Time.deltaTime;

        if (currentDecayDelay <= 0)
        {
            // Gradually reduce the noise level toward zero
            if (currentNoiseRange > 0)
            {
                currentNoiseRange = Mathf.Max(currentNoiseRange - noiseDecayRate * Time.deltaTime, 0);

                if (currentNoiseRange == 0)
                {
                    isActive = false; // Stop updating when noise completely decays
                }
            }
            else
            {
                currentNoiseRange = 0;
                isActive = false;
            }
        }

        TriggerNoiseProcessing();
    }

    public void EmitNoise()
    {
        EmitNoise(noiseDecayRate);
    }

    public void EmitNoise(float intensity)
    {
        if (currentNoiseRange < maxNoiseRange)
        {
            currentNoiseRange = Mathf.Min(currentNoiseRange + intensity * Time.deltaTime, maxNoiseRange);
            currentDecayDelay = noiseDecayDelay;
            isActive = true; // Ensure updates continue when noise is emitted
        }
    }

    public void EmitNoiseImpulse(float impulseNoise)
    {
        currentNoiseRange += impulseNoise;
        wasImpulseNoise = true;
        isActive = true; // Ensure updates continue when impulse noise is emitted
        TriggerNoiseProcessing();
    }

    private void TriggerNoiseProcessing()
    {
        if (currentNoiseRange <= 0) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, currentNoiseRange);
        foreach (Collider collider in colliders)
        {
            GuardHear guardHear = collider.GetComponent<GuardHear>();
            if (guardHear != null)
            {
                guardHear.ProcessNoise(transform, currentNoiseRange, WasImpulseNoise);
            }
        }
    }

    public void SetMaxNoiseRange(float newMaxNoiseRange)
    {
        maxNoiseRange = newMaxNoiseRange;
    }

    private void OnDrawGizmos()
    {
        // Draw the maximum noise range as a red sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxNoiseRange);

        // Draw the current noise range as a yellow sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, currentNoiseRange);
    }
}
