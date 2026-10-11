using System.Collections.Generic;
using UnityEngine;

public class CitizenSpawner : MonoBehaviour
{
    [Header("Spawner Setup")]
    [SerializeField] private GameObject citizenPrefab;
    [SerializeField] private Transform[] possibleSpawnPoints; // Ilagay dito ang lahat ng spawn markers sa map
    [SerializeField] private int numberOfCitizensToSpawn = 3;

    private void Start()
    {
        SpawnCitizens();
    }

    private void SpawnCitizens()
    {
        if (citizenPrefab == null || possibleSpawnPoints.Length == 0) return;

        // Tiyaking hindi hihigit ang bilang sa dami ng markers
        int spawnCount = Mathf.Min(numberOfCitizensToSpawn, possibleSpawnPoints.Length);
        List<Transform> availablePoints = new List<Transform>(possibleSpawnPoints);

        for (int i = 0; i < spawnCount; i++)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[randomIndex];

            Instantiate(citizenPrefab, spawnPoint.position, Quaternion.identity);

            // Alisin para walang dalawang citizen sa iisang spot
            availablePoints.RemoveAt(randomIndex);
        }
    }
}