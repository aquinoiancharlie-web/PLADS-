using UnityEngine;

public class VictimRescueTrigger : MonoBehaviour
{
    [Header("Target Setup")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float rescueRadius = 3f;

    private bool isRescued = false;

    private void Start()
    {
        // Kapag walang in-assign sa Inspector, hahanapin ang Player sa scene
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj == null) playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogError("RESCUE ERROR: Hindi mahanap ang Player object! Paki-drag ang Player sa Inspector slot.");
            }
        }
    }

    private void Update()
    {
        if (isRescued || playerTransform == null) return;

        // Kukuha ng distansya sa pagitan ng Citizen at Player
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Guhitan ng red line sa Scene View
        Debug.DrawLine(transform.position, playerTransform.position, Color.red);

        // Kapag pumasok sa radius ang bangka
        if (distance <= rescueRadius)
        {
            ExecuteRescue();
        }
    }

    /// <summary>
    /// Function na tatawagin ng Salbabida o Proximity para i-rescue ang citizen.
    /// Nagbabalik ng true kapag matagumpay (at hindi pa na-rescue dati).
    /// </summary>
    public bool TryRescue()
    {
        if (isRescued) return false;

        ExecuteRescue();
        return true;
    }

    private void ExecuteRescue()
    {
        if (isRescued) return;
        isRescued = true;

        Debug.Log("SUCCESS: Na-rescue ang Citizen!");

        // Tawagin ang UIManager para sa counter at bonus time
        if (UIManager.Instance != null)
        {
            UIManager.Instance.AddRescuedVictim();
        }
        else
        {
            Debug.LogError("UI ERROR: Walang UIManager na nakatayo sa scene! Siguraduhing may UIManager object sa Hierarchy.");
        }

        // I-hide ang buong citizen object
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isRescued) return;

        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            ExecuteRescue();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rescueRadius);
    }
}