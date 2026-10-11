using UnityEngine;

public class MouseLookTopDown : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private LayerMask groundLayer; // Optional layer filter for ground/water

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        RotateToMouse();
    }

    private void RotateToMouse()
    {
        if (mainCamera == null) return;

        // 1. Cast a ray from the camera through the screen mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position); // Invisible horizontal plane at player's height

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            // 2. Get the exact 3D point where the ray hits the plane
            Vector3 targetPoint = ray.GetPoint(rayDistance);

            // 3. Calculate direction vector from player to mouse point
            Vector3 lookDirection = targetPoint - transform.position;
            lookDirection.y = 0f; // Keep rotation strictly horizontal (no tilting up/down)

            // 4. Smoothly rotate character towards target direction
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}