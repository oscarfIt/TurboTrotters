using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public int maxToSpawn;
    public float radius = 10f; // Set this in the editor
    private LayerMask groundLayer;

    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
        Spawn();
    }

    public void Spawn()
    {
        int amount = maxToSpawn;
        Vector3 spawnPosition;
        for (int i = 0; i < amount; i++)
        {
            spawnPosition = GetSpawnPosition();
            if (spawnPosition != Vector3.zero)
            {
                Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                Debug.Log($"Spawned {prefabToSpawn.name} at {spawnPosition}");
            }
        }
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