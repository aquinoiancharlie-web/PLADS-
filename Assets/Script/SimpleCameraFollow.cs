using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;          // I-drag dito ang BoatPlayer o Player
    [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -6f);

    [Header("Rotation Settings")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float minYAngle = -10f;
    [SerializeField] private float maxYAngle = 60f;

    [Header("Smoothness Settings")]
    [SerializeField] private float smoothSpeed = 0.125f; // Bilis ng pag-follow ng camera

    [Header("Control Mode")]
    [Tooltip("I-check ito kung gusto mong iikot ng Mouse ang mismong Player/Boat pati ang Camera")]
    [SerializeField] private bool rotateTargetWithMouse = false;

    private float currentX = 0f;
    private float currentY = 15f;
    private Vector3 currentVelocity; // Inilagay na rito ang nawawalang variable

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
            currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Kung naka-check ang rotateTargetWithMouse, iikot din ang Player/Boat sa Y-axis
        if (rotateTargetWithMouse)
        {
            target.rotation = Quaternion.Euler(0f, currentX, 0f);
        }

        // Kalkulahin ang position at rotation ng camera
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);
        Vector3 targetPosition = target.position + rotation * offset;

        // Swabeng i-mo-move ang camera
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothSpeed);

        // Nakatutok ang camera sa target
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}