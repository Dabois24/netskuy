using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DetectionIndicator : MonoBehaviour
{
    [SerializeField] private Image frame;
    [SerializeField] private Image detectionBar;

    private void LateUpdate() {
        transform.LookAt(transform.position + Camera.main.transform.forward, Camera.main.transform.rotation * Vector3.up);
    }

    public void UpdateDetectionBar(float current, float max)
    {
        detectionBar.DOFillAmount(current / max, Time.deltaTime);
    }
    public void UpdateDetectionLoseBar(float current, float max)
    {
        detectionBar.DOFillAmount((max - current) / max, Time.deltaTime);
    }
}