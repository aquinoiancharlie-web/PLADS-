using UnityEngine;

public class BoatInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private Transform seatPoint;
    [SerializeField] private Transform disembarkPoint;

    [Header("Script References")]
    [SerializeField] private SimpleBoatController boatMovementScript;

    private SimpleCameraFollow cameraFollow;
    private bool isPlayerNearby = false;
    private bool isBoarded = false;

    private Transform nearbyPlayerTransform;
    private Rigidbody playerRb;
    private Collider playerCollider;
    private MonoBehaviour playerMovementScript;

    private void Start()
    {
        if (Camera.main != null)
        {
            cameraFollow = Camera.main.GetComponent<SimpleCameraFollow>();
        }

        if (boatMovementScript == null)
        {
            boatMovementScript = GetComponent<SimpleBoatController>();
        }
    }

    private void Update()
    {
        // Kapag pumasok ang player sa trigger area at pinindot ang E
        if (Input.GetKeyDown(interactKey))
        {
            if (!isBoarded && isPlayerNearby)
            {
                BoardBoat();
            }
            else if (isBoarded)
            {
                ExitBoat();
            }
            else if (!isPlayerNearby)
            {
                Debug.LogWarning("BOARDING LOG: Pinipindot ang E pero HINDI malapit ang Player sa bangka trigger!");
            }
        }
    }

    private void BoardBoat()
    {
        if (nearbyPlayerTransform == null) return;

        isBoarded = true;

        // Kunan ng components ang Player
        playerRb = nearbyPlayerTransform.GetComponent<Rigidbody>();
        playerCollider = nearbyPlayerTransform.GetComponent<Collider>();
        
        // Isara ang movement script ng player (halimbawa: PlayerController)
        playerMovementScript = nearbyPlayerTransform.GetComponent<MonoBehaviour>();

        if (playerRb != null) playerRb.isKinematic = true;
        if (playerCollider != null) playerCollider.enabled = false;
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        // I-attach sa upuan ng bangka
        nearbyPlayerTransform.SetParent(transform);
        if (seatPoint != null)
        {
            nearbyPlayerTransform.position = seatPoint.position;
            nearbyPlayerTransform.rotation = seatPoint.rotation;
        }

        // I-activate ang bangka
        if (boatMovementScript != null)
        {
            boatMovementScript.SetBoardedState(true);
        }

        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(transform);
        }

        Debug.Log("BOARDING LOG: Matagumpay na nakasakay ang Player!");
    }

    private void ExitBoat()
    {
        if (nearbyPlayerTransform == null) return;

        isBoarded = false;

        // Alisin ang player bilang child
        nearbyPlayerTransform.SetParent(null);

        if (disembarkPoint != null)
        {
            nearbyPlayerTransform.position = disembarkPoint.position;
            nearbyPlayerTransform.rotation = disembarkPoint.rotation;
        }

        // Ibalik ang player physics at movement
        if (playerRb != null) playerRb.isKinematic = false;
        if (playerCollider != null) playerCollider.enabled = true;
        if (playerMovementScript != null) playerMovementScript.enabled = true;

        // I-disable ang bangka controls
        if (boatMovementScript != null)
        {
            boatMovementScript.SetBoardedState(false);
        }

        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(nearbyPlayerTransform);
        }

        Debug.Log("BOARDING LOG: Bumaba na ang Player sa bangka!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            nearbyPlayerTransform = other.transform;
            Debug.Log("BOARDING LOG: Lumapit na ang Player sa bangka! Maaari nang pumindot ng E.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isBoarded)
        {
            isPlayerNearby = false;
            Debug.Log("BOARDING LOG: Lumayo na ang Player sa bangka.");
        }
    }
}