using UnityEngine;

public class LeaderTracker : MonoBehaviour
{
    public GameObject currentLeader;
    public GameObject centreObject; // Set this in the editor
    public Vector3 trackCentre;
    private string currentTrackSection;
    void Start()
    {
        if (centreObject == null)
            trackCentre = new Vector3(250, 25, 150); // This default is for Map1
        else
            trackCentre = centreObject.transform.position;
        currentTrackSection = TrackSection.SouthStraight;       // Adjust this if we need to start in a different section
    }

    void Update()
    {
        if (currentLeader == null) return;
        UpdatePlaneOrientation();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Player {other.gameObject.name} entered trigger");
            SetNewLeader(other.gameObject);
        }
        else if (other.gameObject.CompareTag(TrackSection.SouthStraight) ||
                 other.gameObject.CompareTag(TrackSection.EastStraight) ||
                 other.gameObject.CompareTag(TrackSection.NorthStraight) ||
                 other.gameObject.CompareTag(TrackSection.WestStraight) ||
                 other.gameObject.CompareTag(TrackSection.SecantNorthEast) ||
                 other.gameObject.CompareTag(TrackSection.SecantNorthWest) ||
                 other.gameObject.CompareTag(TrackSection.SecantSouthEast) ||
                 other.gameObject.CompareTag(TrackSection.SecantSouthWest))
        {
            Debug.Log($"Plane {gameObject.name} entered track section: {other.gameObject.tag}");
            currentTrackSection = other.gameObject.tag;
        }
    }

    private void SetNewLeader(GameObject newLeader)
    {
        if (currentLeader != null)
        {
            Debug.Log($"Detaching from current leader: {currentLeader.name}");
            // Move the plane a small distance away from the current leader to avoid immediate collision
            // This will naturally be in the direction of the new leader
            Vector3 incrementVector = GetIncrementVector(transform.position, newLeader.transform.position);
            // Detach from the current leader
            currentLeader = null;
            transform.SetParent(null);
            transform.position += incrementVector;
            // Return and wait for the next trigger
            return;
        }

        currentLeader = newLeader;
        transform.SetParent(currentLeader.transform);

        // Reset the object's position relative to the new player
        transform.localPosition = Vector3.zero; // You can modify this to have an offset if needed
    }


    private Vector3 GetIncrementVector(Vector3 planePosition, Vector3 incomingPlayerPosition)
    {
        Vector3 incrementVector = Vector3.zero;
        switch (currentTrackSection)
        {
            case TrackSection.SouthStraight:
                incrementVector.x += 1f;
                break;
            case TrackSection.EastStraight:
                incrementVector.z += 1f;
                break;
            case TrackSection.NorthStraight:
                incrementVector.x -= 1f;
                break;
            case TrackSection.WestStraight:
                incrementVector.z -= 1f;
                break;
            case TrackSection.SecantNorthEast:   // Hard coded for a NorthEast secant for now
                incrementVector.x += 1f;
                incrementVector.z += 1f;
                break;
            case TrackSection.SecantNorthWest:
                incrementVector.x -= 1f;
                incrementVector.z += 1f;
                break;
            case TrackSection.SecantSouthEast:
                incrementVector.x += 1f;
                incrementVector.z -= 1f;
                break;
            case TrackSection.SecantSouthWest:
                incrementVector.x -= 1f;
                incrementVector.z -= 1f;
                break;
            default:
                Debug.LogWarning("Unknown track section, no increment vector applied.");
                break;
        }
        return incrementVector;
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
            case TrackSection.SecantNorthEast:
            case TrackSection.SecantNorthWest:
            case TrackSection.SecantSouthEast:
            case TrackSection.SecantSouthWest:
                Vector3 toCenter = (trackCentre - transform.position).normalized;
                // Normal vector to the plane
                Vector3 normal = Vector3.Cross(toCenter, Vector3.up).normalized;
                transform.rotation = Quaternion.LookRotation(toCenter, normal);
                break;
        }
    }
}
