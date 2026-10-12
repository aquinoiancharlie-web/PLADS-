using UnityEngine;

public class SafeZone : MonoBehaviour
{
    [Header("Rescue Settings")]
    [SerializeField] private int totalCitizensToRescue = 2;

    private int rescuedCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        // Tinitingnan kung Player o Bangka ang pumasok sa Safezone
        if (other.CompareTag("Player") || other.name.ToLower().Contains("boat") || other.transform.root.CompareTag("Player"))
        {
            CheckAndDeliverCitizen(other.gameObject);
        }
    }

    private void CheckAndDeliverCitizen(GameObject entrant)
    {
        // Gagamitin natin ang VictimRescueTrigger sa halip na "Citizen"
        VictimRescueTrigger victim = entrant.GetComponentInChildren<VictimRescueTrigger>();

        if (victim != null)
        {
            // Isasagawa ang rescue sa victim
            if (victim.TryRescue())
            {
                rescuedCount++;
                Debug.Log($"Citizen Delivered to Safezone! Total: {rescuedCount}/{totalCitizensToRescue}");

                if (rescuedCount >= totalCitizensToRescue)
                {
                    OnMissionComplete();
                }
            }
        }
    }

    private void OnMissionComplete()
    {
        Debug.Log("Mission Accomplished! Lahat ng citizen ay ligtas na sa SafeZone!");
    }
}