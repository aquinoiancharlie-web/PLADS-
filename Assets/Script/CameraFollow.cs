using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform target; // Dito i-da-drag ang Player object

    [Header("Isometric Position Offset")]
    public Vector3 offset = new Vector3(0f, 12f, -10f); // Taas at layo ng camera

    [Header("Camera Settings")]
    public float smoothSpeed = 10f;
    public float pitchAngle = 50f;

    private void Start()
    {
        // Set fixed downward angle para sa top-down view
        transform.rotation = Quaternion.Euler(pitchAngle, 0f, 0f);
    }

    private void LateUpdate()
    {
        // Kung walang player na naka-assign, huwag muna mag-run
        if (target == null) return;

        // Kunin ang gustong posisyon batay sa player
        Vector3 targetPosition = target.position + offset;

        // Swabeng galaw papunta sa targetPosition
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}