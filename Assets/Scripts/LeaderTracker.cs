using UnityEngine;

public class LeaderTracker : MonoBehaviour
{
    public GameObject currentLeader;
    public Vector3 trackCentre;
    private string currentTrackSection;
    void Start()
    {
        trackCentre = GameObject.FindWithTag("TrackCentre").transform.position;
        currentTrackSection = TrackSection.SouthStraight;       // Adjust this if we need to start in a different section
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
            Vector3 directionToPlayer = (other.transform.position - transform.position);
            Debug.Log($"Player {other.gameObject.name} entered trigger");
            Debug.Log($"Direction to player: {directionToPlayer}");
            SetNewLeader(other.gameObject);
        }
        else if (other.gameObject.CompareTag(TrackSection.SouthStraight) ||
                 other.gameObject.CompareTag(TrackSection.EastStraight) ||
                 other.gameObject.CompareTag(TrackSection.NorthStraight) ||
                 other.gameObject.CompareTag(TrackSection.WestStraight) ||
                 other.gameObject.CompareTag(TrackSection.Secant))
        {
            Debug.Log($"Plane {gameObject.name} entered track section: {other.gameObject.tag}");
            currentTrackSection = other.gameObject.tag;
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
    private void UpdatePlaneOrientation()
    {
        switch (currentTrackSection)
        {
            case TrackSection.SouthStraight:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case TrackSection.EastStraight:
                transform.rotation = Quaternion.Euler(0, -90, -90);
                break;
            case TrackSection.NorthStraight:
                transform.rotation = Quaternion.Euler(0, -180, -90);
                break;
            case TrackSection.WestStraight:
                transform.rotation = Quaternion.Euler(0, -270, -90);
                break;
            case TrackSection.Secant:
                Vector3 toCenter = (trackCentre - transform.position).normalized;
                // Normal vector to the plane
                Vector3 normal = Vector3.Cross(toCenter, Vector3.up).normalized;
                transform.rotation = Quaternion.LookRotation(toCenter, normal);
                break;
        }
    }
}
