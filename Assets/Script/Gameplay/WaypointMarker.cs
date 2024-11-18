using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaypointMarker : MonoBehaviour
{
    public Image WaypointImage;
    public Transform Target;
    public TMP_Text DistanceText;
    public Vector3 Offset;
    private Transform player;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        float minX = WaypointImage.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = WaypointImage.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 pos = Camera.main.WorldToScreenPoint(Target.position + Offset);

        if (Vector3.Dot((Target.position - transform.position), transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            }
            else
            {
                pos.x = minX;
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        WaypointImage.transform.position = pos;
        DistanceText.text = ((int)Vector3.Distance(Target.position, player.position)).ToString() + "m";
    }
}
