using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SalbabidaProjectile : MonoBehaviour
{
    [Header("Flight & Arc Settings")]
    [SerializeField] private float flySpeed = 15f;
    [SerializeField] private float arcHeight = 3f; 
    [SerializeField] private float pullSpeed = 12f;
    [SerializeField] private float rescueRadius = 2.5f;
    [SerializeField] private float onboardDistance = 0.8f;

    [Header("Visual Effects")]
    [SerializeField] private LineRenderer ropeRenderer;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Transform playerTransform;
    private Transform hookedCitizen = null;

    private float flightProgress = 0f;
    private float totalFlightTime = 1f;

    private enum State { Flying, Pulling, Completed }
    private State currentState = State.Flying;

    private bool hasTriggeredRescue = false;

    private void Awake()
    {
        if (ropeRenderer == null)
        {
            ropeRenderer = GetComponent<LineRenderer>();
        }

        if (ropeRenderer != null)
        {
            ropeRenderer.positionCount = 2;
            ropeRenderer.startWidth = 0.08f;
            ropeRenderer.endWidth = 0.08f;
            ropeRenderer.enabled = false;
        }
    }

    public void Launch(Vector3 target, Transform player)
    {
        startPosition = transform.position;
        targetPosition = target;
        playerTransform = player;
        currentState = State.Flying;

        float distance = Vector3.Distance(startPosition, targetPosition);
        totalFlightTime = Mathf.Max(0.1f, distance / flySpeed);
        flightProgress = 0f;

        if (ropeRenderer != null)
        {
            ropeRenderer.enabled = true;
        }

        Invoke(nameof(CheckFlightTimeout), 8f);
    }

    private void CheckFlightTimeout()
    {
        if (currentState == State.Flying && hookedCitizen == null)
        {
            ClearLineRenderer();
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdateRopeVisual();

        switch (currentState)
        {
            case State.Flying:
                HandleParabolicFlight();
                CheckForHook();
                break;

            case State.Pulling:
                HandlePulling();
                break;

            case State.Completed:
                break;
        }
    }

    private void HandleParabolicFlight()
    {
        flightProgress += Time.deltaTime / totalFlightTime;

        if (flightProgress <= 1f)
        {
            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, flightProgress);
            float heightOffset = Mathf.Sin(flightProgress * Mathf.PI) * arcHeight;
            currentPos.y += heightOffset;

            transform.position = currentPos;
        }
        else
        {
            transform.position = targetPosition;

            if (hookedCitizen == null)
            {
                ClearLineRenderer();
                Destroy(gameObject, 0.5f);
            }
        }
    }

    private void HandlePulling()
    {
        if (hookedCitizen != null && playerTransform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, pullSpeed * Time.deltaTime);
            hookedCitizen.position = transform.position;

            if (Vector3.Distance(transform.position, playerTransform.position) <= onboardDistance)
            {
                CompleteRescue();
            }
        }
        else
        {
            ClearLineRenderer();
            Destroy(gameObject);
        }
    }

    private void CheckForHook()
    {
        if (hookedCitizen != null || currentState != State.Flying) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, rescueRadius);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Citizen") || hit.transform.root.CompareTag("Citizen") || hit.name.ToLower().Contains("citizen"))
            {
                hookedCitizen = hit.transform.root;
                currentState = State.Pulling;

                CancelInvoke(nameof(CheckFlightTimeout));

                // I-disable ang Collider ng Citizen para HINDI na mag-trigger sa ibang scripts (tulad ng VictimRescueTrigger)
                Collider citizenCollider = hookedCitizen.GetComponentInChildren<Collider>();
                if (citizenCollider != null)
                {
                    citizenCollider.enabled = false;
                }

                Rigidbody citizenRb = hookedCitizen.GetComponent<Rigidbody>();
                if (citizenRb != null)
                {
                    citizenRb.isKinematic = true;
                }
                break;
            }
        }
    }

    private void UpdateRopeVisual()
    {
        if (ropeRenderer != null && ropeRenderer.enabled && playerTransform != null)
        {
            ropeRenderer.positionCount = 2;
            ropeRenderer.SetPosition(0, playerTransform.position);
            ropeRenderer.SetPosition(1, transform.position);
        }
    }

    private void CompleteRescue()
    {
        // Guard flag para masiguradong 1 beses lang tatakbo ang rescue logic
        if (hasTriggeredRescue) return;
        hasTriggeredRescue = true;

        currentState = State.Completed;
        ClearLineRenderer();

        if (hookedCitizen != null)
        {
            // Itago ang Citizen at dagdagan ng eksaktong 1 ang rescued count
            hookedCitizen.gameObject.SetActive(false);

            if (UIManager.Instance != null)
            {
                UIManager.Instance.AddRescuedVictim();
            }
        }

        Destroy(gameObject);
    }

    private void ClearLineRenderer()
    {
        if (ropeRenderer != null)
        {
            ropeRenderer.positionCount = 0;
            ropeRenderer.enabled = false;
        }
    }

    private void OnDestroy()
    {
        ClearLineRenderer();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rescueRadius);
    }
}