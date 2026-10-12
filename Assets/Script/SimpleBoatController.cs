using UnityEngine;

public class SimpleBoatController : MonoBehaviour
{
    [Header("Boat Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;      // Bilis ng pag-andar forward/backward
    [SerializeField] private float turnSpeed = 80f;      // Bilis ng pagliko/pag-ikot

    [Header("Inversion Options")]
    [SerializeField] private bool invertMovement = false; // Check kung baliktad ang W/S
    [SerializeField] private bool invertTurning = false;  // Check kung baliktad ang A/D

    [Header("Boarding State")]
    [SerializeField] private bool isBoarded = false;

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("BOAT ERROR: Walang Rigidbody na nakakabit sa Game Object na ito!");
            return;
        }

        // Tiyaking FreezePositionY, FreezeRotationX, at FreezeRotationZ LAMANG ang naka-lock.
        // HUWAG I-LOCK ANG ROTATION Y para makaliko ang bangka!
        rb.constraints = RigidbodyConstraints.FreezePositionY | 
                         RigidbodyConstraints.FreezeRotationX | 
                         RigidbodyConstraints.FreezeRotationZ;

        // Siguraduhing Unchecked ang IsKinematic para sa physics movement
        rb.isKinematic = false;
    }

    private void Update()
    {
        if (!isBoarded)
        {
            moveInput = 0f;
            turnInput = 0f;
            return;
        }

        // Basahin ang input ng player
        moveInput = Input.GetAxisRaw("Vertical");    // W (+1), S (-1)
        turnInput = Input.GetAxisRaw("Horizontal");  // D (+1), A (-1)
    }

    private void FixedUpdate()
    {
        if (rb == null || !isBoarded) return;

        // 1. PAG-IKOT / TURNING (A/D Keys)
        if (turnInput != 0)
        {
            float turnMultiplier = invertTurning ? -1f : 1f;
            float turnAmount = turnInput * turnSpeed * turnMultiplier * Time.fixedDeltaTime;

            // Direct rotation sa Y axis para siguradong umiikot saan man nakatutok
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // 2. PAG-ANDAR / MOVEMENT (W/S Keys)
        if (moveInput != 0)
        {
            float moveMultiplier = invertMovement ? -1f : 1f;

            // Gamitin ang mismong KASALUKUYANG forward vector ng bangka (Local Forward)
            // Para kahit saan nakatutok ang bangka, papunta roon ang forward movement ng W key
            Vector3 forwardDir = transform.forward;
            forwardDir.y = 0f; // Manatiling pantay sa tubig
            forwardDir.Normalize();

            Vector3 moveVector = forwardDir * (moveInput * moveMultiplier) * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(rb.position + moveVector);
        }
    }

    public void SetBoardedState(bool state)
    {
        isBoarded = state;

        if (!isBoarded)
        {
            moveInput = 0f;
            turnInput = 0f;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}