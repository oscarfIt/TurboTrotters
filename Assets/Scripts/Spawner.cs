using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public List<GameObject> spawnedObjects;
    public int maxToSpawn;
    public float radius = 10f; // Set this in the editor
    private LayerMask groundLayer;
    private int numToSpawn;

    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
        spawnedObjects = new List<GameObject>();
        numToSpawn = maxToSpawn;
        Spawn();
    }

    public void Spawn()
    {
        Vector3 spawnPosition;
        for (int i = 0; i < numToSpawn; i++)
        {
            spawnPosition = GetSpawnPosition();
            if (spawnPosition != Vector3.zero)
            {
                spawnedObjects.Add(Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity));
                Debug.Log($"Spawned {prefabToSpawn.name} at {spawnPosition}");
            }
            else
            {
                Debug.LogWarning($"Failed to spawn {prefabToSpawn.name} at random position. Position was zero.");
            }
        }
        Debug.Log($"Spawned {numToSpawn} objects of {prefabToSpawn.name}");
    }

    public void NewLapCleanup()
    {
        numToSpawn = 0;
        Debug.Log($"New lap cleanup for {prefabToSpawn.name}, count = {spawnedObjects.Count}");
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] == null)
            {
                Debug.Log($"Object {i} is null, incrementing objects to spawn.");
                numToSpawn++;
            }
            else
            {
                Debug.Log($"Objecct at {i} is {spawnedObjects[i].name}");
            }
        }
        spawnedObjects.RemoveAll(obj => obj == null);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        Vector3 randomPosition = new Vector3(transform.position.x + randomCircle.x, transform.position.y + 10f, transform.position.z + randomCircle.y); // Start raycast above the spawner
        if (Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}