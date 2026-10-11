using UnityEngine;

public class SimpleBoatController : MonoBehaviour
{
    [Header("Boat Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;     // Bilis ng pag-andar forward/backward
    [SerializeField] private float turnSpeed = 100f;   // Bilis ng pagikot/pagliko

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Tiyaking hindi guguho o tatagilid ang 3D block
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    private void Update()
    {
        // W/S o Up/Down Arrow Keys para sa forward/backward
        moveInput = Input.GetAxisRaw("Vertical");

        // A/D o Left/Right Arrow Keys para sa pagliko
        turnInput = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        // 1. PAG-IKOT (TURNING)
        if (turnInput != 0)
        {
            float turnAmount = turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // 2. PAG-ANDAR (FORWARD / BACKWARD MOVEMENT)
        if (moveInput != 0)
        {
            // Aandar papunta sa DIREKSYON NG HARAP NG BANGKA (transform.forward)
            Vector3 moveDirection = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + moveDirection);
        }
    }
}