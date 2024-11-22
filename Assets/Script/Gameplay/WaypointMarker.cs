using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WaypointMarker : MonoBehaviour
{
    [SerializeField] private Image waypointImage;
    [SerializeField] private Image bar;
    [SerializeField] private Transform target;
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private Vector3 offset;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        UpdateWaypointPosition();
        UpdateDistanceText();
    }

    private void UpdateWaypointPosition()
    {
        float minX = waypointImage.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = waypointImage.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector3 screenPos = Camera.main.WorldToViewportPoint(target.position + offset);

        if (screenPos.z < 0)
        {
            screenPos.x = 1f - screenPos.x;
            screenPos.y = 1f - screenPos.y;
            screenPos.z = 0;
        }

        Vector2 pos = Camera.main.ViewportToScreenPoint(new Vector2(screenPos.x, screenPos.y));
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        waypointImage.transform.position = pos;
    }

    private void UpdateDistanceText()
    {
        distanceText.text = ((int)Vector3.Distance(target.position, player.position)).ToString() + "m";
    }

    public void UpdateBar(float current, float max)
    {
        bar.DOFillAmount(current / max, 0.1f); // Smoothly animate the bar fill
    }
}
