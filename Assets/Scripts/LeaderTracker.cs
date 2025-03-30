using UnityEngine;

public class LeaderTracker : MonoBehaviour
{
    public GameObject currentLeader;
    public Vector3 trackCentre;

    void Start()
    {
        trackCentre = GameObject.FindWithTag("TrackCentre").transform.position;
    }

    void Update()
    {
        if (currentLeader == null) return;

        Vector3 toCenter = (trackCentre - transform.position).normalized;
        // Normal vector to the plane
        Vector3 normal = Vector3.Cross(toCenter, Vector3.up).normalized;
        transform.rotation = Quaternion.LookRotation(toCenter, normal);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SetNewLeader(other.gameObject);
        }
    }

    private void SetNewLeader(GameObject newLeader)
    {
        if (currentLeader != null)
        {
            transform.SetParent(null);
        }

        currentLeader = newLeader;
        transform.SetParent(currentLeader.transform);

        // Reset the object's position relative to the new player
        transform.localPosition = Vector3.zero; // You can modify this to have an offset if needed
    }
}
