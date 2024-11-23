using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Clock : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private RectTransform hourHand;
    [SerializeField] private RectTransform minuteHand;
    [SerializeField] private RectTransform secondHand;

    [Header("Values")]
    [SerializeField] private float gameStartHour = 9f; 
    [SerializeField] private float schoolEndHour = 15f;
    [SerializeField] private float gamePlayTime = 300f;

    private const float HoursInClock = 12f;
    private const float DegreesPerHour = 360f / HoursInClock;
    private const float DegreesPerMinute = 360f / 60f;
    private const float DegreesPerSecond = 360f / 60f;

    private Tween hourTween;
    private Tween minuteTween;
    private Tween secondTween;

    private void Start()
    {
        AnimateClockHands();
    }

    private void AnimateClockHands()
    {
        // Hour Hand Calculation
        float startHourAngle = gameStartHour % HoursInClock * DegreesPerHour;
        float endHourAngle = schoolEndHour % HoursInClock * DegreesPerHour;
        if (endHourAngle <= startHourAngle)
        {
            endHourAngle += 360f;
        }
        float totalHourRotation = endHourAngle - startHourAngle;

        // Set initial rotations
        hourHand.rotation = Quaternion.Euler(0, 0, -startHourAngle);
        minuteHand.rotation = Quaternion.identity;
        secondHand.rotation = Quaternion.identity;

        // Hour Hand Animation
        hourTween = hourHand
            .DORotate(new Vector3(0, 0, -startHourAngle - totalHourRotation), gamePlayTime, RotateMode.Fast)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                GameManager.Instance?.ChangeState(GameManager.GameState.TimeUp);
            });

        // Minute Hand Animation (rotates 360 degrees per hour of gameplay)
        float totalMinuteRotation = 360f * (schoolEndHour - gameStartHour);
        minuteTween = minuteHand
            .DORotate(new Vector3(0, 0, -totalMinuteRotation), gamePlayTime, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);

        // Second Hand Animation (rotates 360 degrees per minute of gameplay)
        float totalSecondRotation = 360f * 60f * (schoolEndHour - gameStartHour);
        secondTween = secondHand
            .DORotate(new Vector3(0, 0, -totalSecondRotation), gamePlayTime, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);
    }

    public void StopClockRotation()
    {
        // Stop all animations
        if (hourTween != null && hourTween.IsActive())
        {
            hourTween.Kill(false);
            hourTween = null;
        }

        if (minuteTween != null && minuteTween.IsActive())
        {
            minuteTween.Kill(false);
            minuteTween = null;
        }

        if (secondTween != null && secondTween.IsActive())
        {
            secondTween.Kill(false);
            secondTween = null;
        }
    }
}
