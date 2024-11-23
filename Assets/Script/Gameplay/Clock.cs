using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Clock : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private Image clockHand;

    [Header("Values")]
    [SerializeField] private float gameStartHour = 9f; 
    [SerializeField] private float schoolEndHour = 15f;
    [SerializeField] private float gamePlayTime = 300f;

    private const float HoursInClock = 12f;
    private const float DegreesPerHour = 360f / HoursInClock;

    private Tween clockTween;

    private void Start()
    {
        AnimateClockHand();
    }

    private void AnimateClockHand()
    {
        float startAngle = gameStartHour % HoursInClock * DegreesPerHour;
        float endAngle = schoolEndHour % HoursInClock * DegreesPerHour;

        if (endAngle <= startAngle)
        {
            endAngle += 360f;
        }

        clockHand.rectTransform.rotation = Quaternion.Euler(0, 0, -startAngle);

        float totalRotation = endAngle - startAngle;

        clockTween = clockHand.rectTransform
            .DORotate(new Vector3(0, 0, -startAngle - totalRotation), gamePlayTime, RotateMode.Fast)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                GameManager.Instance?.ChangeState(GameManager.GameState.TimeUp);
            });
    }

    public void StopClockRotation()
    {
        if (clockTween != null && clockTween.IsActive())
        {
            clockTween.Kill(false);
            clockTween = null;
        }
    }
}
