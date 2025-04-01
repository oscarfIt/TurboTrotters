using UnityEngine;
using System.Collections;

public class BarrelSpawner : MonoBehaviour
{
    public GameObject barrelPrefab; // Assign the barrel prefab in the Inspector
    public Transform[] spawnPoints; // Assign hilltop spawn points in the Inspector
    public float spawnRadius = 2f; // Radius around each spawn point
    public float spawnInterval = 3f; // Time between spawns

    void Start()
    {
        StartCoroutine(SpawnBarrels());
    }

    IEnumerator SpawnBarrels()
    {
        while (true)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                Vector3 randomOffset = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPosition = new Vector3(
                    spawnPoint.position.x + randomOffset.x,
                    spawnPoint.position.y,
                    spawnPoint.position.z + randomOffset.y
                );

                Instantiate(barrelPrefab, spawnPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}