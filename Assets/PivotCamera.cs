using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform player;  // Assign the player in the Inspector
    public float orbitSpeed = 30f;  // Speed of orbiting
    public float followSpeed = 5f;  // Smooth transition speed
    public float distanceFromCenter = 20f; // Distance from the map center
    public float heightOffset = 5f; // Fixed height above ground

    void LateUpdate()
    {
        if (player == null) return;

        // Step 1: Get direction from map center to player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Step 2: Calculate the new camera position (orbiting the center)
        Quaternion orbitRotation = Quaternion.Euler(0, orbitSpeed * Time.deltaTime, 0);
        Vector3 newCameraPosition = orbitRotation * (transform.GetChild(0).position - transform.position) + transform.position;

        // Step 3: Apply smoothing
        transform.GetChild(0).position = Vector3.Lerp(transform.GetChild(0).position, newCameraPosition, followSpeed * Time.deltaTime);
        transform.GetChild(0).position = new Vector3(transform.GetChild(0).position.x, heightOffset, transform.GetChild(0).position.z);

        // Step 4: Keep the camera looking at the player
        transform.GetChild(0).LookAt(player);
    }
}
