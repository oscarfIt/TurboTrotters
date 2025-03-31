using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaceManager : MonoBehaviour
{
    private List<GameObject> players;
    private GameObject currentLeader;

    public string currentTrackSection;  // Used pretty often in LeaderTracker.cs


    void Start()
    {
        players = new List<GameObject>();
        currentTrackSection = TrackSection.SouthStraight;       // Adjust this if we need to start in a different section
    }

    void Update()
    {
        
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        player.gameObject.name = "Player_" + player.playerIndex.ToString();
        players.Add(player.gameObject);
    }

    public void SetLeader(GameObject leader)
    {
        if (players.Contains(leader))
        {
            currentLeader = leader;
        }
        else
        {
            Debug.LogError("Attempting to set a non-player object as leader: " + leader.name);  // This seems like a mild error to throw given the circumstances
            return;
        }
    }


    public System.Collections.IEnumerator KickPig(GameObject pigToKick)
    {

        Vector3 startPos = pigToKick.transform.position;
        Vector3 targetPos = currentLeader.transform.position;;

        float elapsedTime = 0f;

        while (elapsedTime < Movement.KICK_DURATION)
        {
            targetPos = currentLeader.transform.position; // Maybe check for null here
            targetPos += GetKickedOffset();
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / Movement.KICK_DURATION;

            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);

            newPos.y += Mathf.Sin(t * Mathf.PI) * Movement.KICKED_HEIGHT;

            pigToKick.transform.position = newPos;

            yield return null;
        }

        pigToKick.transform.position = targetPos;
    }

    public void SetCurrentTrackSection(string newSection)
    {
        currentTrackSection = newSection;
    }

    // Deja vu
    private Vector3 GetKickedOffset()
    {
        Vector3 offsetVetor = Vector3.zero;
        switch (currentTrackSection)
        {
            case TrackSection.SouthStraight:
                offsetVetor.x -= Movement.KICKED_POSITION_OFFSET;
                break;
            case TrackSection.EastStraight:
                offsetVetor.z -= Movement.KICKED_POSITION_OFFSET;
                break;
            case TrackSection.NorthStraight:
                offsetVetor.x += Movement.KICKED_POSITION_OFFSET;
                break;
            case TrackSection.WestStraight:
                offsetVetor.z += Movement.KICKED_POSITION_OFFSET;
                break;
            case TrackSection.SecantNorthEast:
                offsetVetor.x -= Movement.KICKED_POSITION_OFFSET;
                offsetVetor.z -= Movement.KICKED_POSITION_OFFSET;
                break;
            case TrackSection.SecantNorthWest:
                offsetVetor.x += Movement.KICKED_POSITION_OFFSET;
                offsetVetor.z -= Movement.KICKED_POSITION_OFFSET;
                break;
            case TrackSection.SecantSouthEast:
                offsetVetor.x -= Movement.KICKED_POSITION_OFFSET;
                offsetVetor.z += Movement.KICKED_POSITION_OFFSET;
                break;
            case TrackSection.SecantSouthWest:
                offsetVetor.x += Movement.KICKED_POSITION_OFFSET;
                offsetVetor.z += Movement.KICKED_POSITION_OFFSET;
                break;
            default:
                Debug.LogWarning("Unknown track section, no offset vector applied.");
                break;
        }
        return offsetVetor;
    }

}