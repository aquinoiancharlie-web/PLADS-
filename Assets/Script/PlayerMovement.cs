using UnityEngine;
using UnityEngine.EventSystems; // Para sa UI check

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private bool holdToLook = false; // Naka-set sa false para laging sumusunod sa mouse ang tingin

    private Camera mainCamera;
    private Rigidbody rb;
    private Vector3 moveInput;

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 1. Movement Input (WASD / Arrow Keys)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(moveX, 0f, moveZ).normalized;

        // 2. Mouse Look Input (Pag-ikot ng Character papunta sa Mouse Position)
        if (!holdToLook || Input.GetMouseButton(0))
        {
            if (EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject())
            {
                RotateTowardsMouse();
            }
        }
    }

    private void FixedUpdate()
    {
        // Malinis na Physics-based Movement
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
}