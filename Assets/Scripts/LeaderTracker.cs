using UnityEngine;

public class LeaderTracker : MonoBehaviour
{

    public GameObject currentLeader;

    void Start()
    {
        
    }

    void Update()
    {
        Debug.Log($"Current Leader: {currentLeader}");
        Debug.Log($"Position of LeaderTracker = {transform.position}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger");
            if (currentLeader != null)
            {
                transform.SetParent(null);
            }

            currentLeader = other.gameObject;
            transform.SetParent(currentLeader.transform);

            // Reset the object's position relative to the new player
            transform.localPosition = Vector3.zero; // You can modify this to have an offset if needed
            transform.localRotation = Quaternion.identity; // Optionally reset rotation
        }
    }
}
