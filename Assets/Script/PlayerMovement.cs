using UnityEngine;
using UnityEngine.EventSystems; // Para sa UI check

[RequireComponent(typeof(LineRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private bool holdToLook = true;

    [Header("Throwing & Aiming Settings")]
    [SerializeField] private GameObject salbabidaPrefab;  // I-drag ang Salbabida Prefab dito sa Inspector
    [SerializeField] private Transform throwPoint;        // Dito magsisimula ang bato (harapan ng bangka)
    [SerializeField] private int trajectoryPoints = 25;    // Dami ng points ng trajectory line
    [SerializeField] private float arcHeight = 3f;         // Taas ng kurba ng trajectory sa ere

    private Camera mainCamera;
    private Rigidbody rb;
    private Vector3 moveInput;
    private LineRenderer trajectoryRenderer;

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();

        // Tiyaking may LineRenderer para sa Trajectory Arc Visualizer
        trajectoryRenderer = GetComponent<LineRenderer>();
        if (trajectoryRenderer != null)
        {
            trajectoryRenderer.positionCount = trajectoryPoints;
            trajectoryRenderer.startWidth = 0.1f;
            trajectoryRenderer.endWidth = 0.1f;
            trajectoryRenderer.enabled = false;
        }

        if (throwPoint == null) throwPoint = transform;
    }

    private void Update()
    {
        // 1. Movement Input (WASD / Arrow Keys)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(moveX, 0f, moveZ).normalized;

        // 2. Mouse Look Input
        if (!holdToLook || Input.GetMouseButton(0))
        {
            if (EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject())
            {
                RotateTowardsMouse();
            }
        }

        // 3. Realtimely i-update ang Trajectory Arc patungo sa Cursor
        UpdateTrajectoryAim();

        // 4. Bato ng Salbabida sa Right Click (Mouse1)
        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject())
            {
                ThrowSalbabida();
            }
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            transform.Translate(moveInput * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    private void RotateTowardsMouse()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane waterPlane = new Plane(Vector3.up, transform.position);

        if (waterPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 targetPoint = ray.GetPoint(rayDistance);
            Vector3 lookDirection = targetPoint - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void UpdateTrajectoryAim()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null || trajectoryRenderer == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane waterPlane = new Plane(Vector3.up, throwPoint.position);

        if (waterPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 targetPoint = ray.GetPoint(rayDistance);
            Vector3 startPoint = throwPoint.position;

            trajectoryRenderer.enabled = true;

            // Iguguhit ang curved parabolic path papunta sa mouse cursor
            for (int i = 0; i < trajectoryPoints; i++)
            {
                float t = (float)i / (trajectoryPoints - 1);
                Vector3 currentPos = Vector3.Lerp(startPoint, targetPoint, t);

                // Parabolic Arc Height Offset
                float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
                currentPos.y += height;

                trajectoryRenderer.SetPosition(i, currentPos);
            }
        }
    }

    private void ThrowSalbabida()
    {
        if (salbabidaPrefab == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane waterPlane = new Plane(Vector3.up, throwPoint.position);

        if (waterPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 targetPoint = ray.GetPoint(rayDistance);

            GameObject buoy = Instantiate(salbabidaPrefab, throwPoint.position, throwPoint.rotation);
            SalbabidaProjectile projectile = buoy.GetComponent<SalbabidaProjectile>();

            if (projectile != null)
            {
                projectile.Launch(targetPoint, transform);
            }
        }
    }
}