using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaceManager : MonoBehaviour
{
    public List<GameObject> players;
    private GameObject currentLeader;   // Oh the duplication
    private GameObject[] spawners;
    private int currentLap = -1;    // Since players start behind the start line
    public int numLaps; // Set this in the editor

    public string currentTrackSection;  // Used pretty often in LeaderTracker.cs

    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    public Material redMaterial;
    public Material blueMaterial;
    public Material greenMaterial;
    public Material yellowMaterial;
    void Start()
    {
        players = new List<GameObject>();
        currentTrackSection = TrackSection.SouthStraight;       // Adjust this if we need to start in a different section
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        if (numLaps == 0) numLaps = 1;

        var playerDataList = JoinManager.instance.playerDataList;

        for (int i = 0; i < playerDataList.Count; i++) {
            PlayerData data = playerDataList[i];
            Transform spawnPoint = spawnPoints[i];

            PlayerInput player = PlayerInput.Instantiate(
            playerPrefab,
            controlScheme: null,
            pairWithDevice: data.inputDevice
        );

            player.transform.position = spawnPoint.position;

            Renderer rend = player.gameObject.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                Material chosenMat = GetMaterialForColor(data.color);
                if (chosenMat != null)
                {
                    rend.material = chosenMat;
                }
            }
            var playerController = player.GetComponent<pigController>();
            playerController.setPlayerIndex(i);
            //player.gameObject.setPlayerIndex(i);
            // Debug.Log("Player " + i + " Spawn");
            players.Add(player.gameObject);
        }

        FindObjectOfType<RaceHUDManager>().SetupHUD(players);
    }

    void Update()
    {
        
    }

   // public void OnPlayerJoined(PlayerInput player)
   // {
    //    player.gameObject.name = "Player_" + player.playerIndex.ToString();
    //    players.Add(player.gameObject);
  //  }

    public void SetLeader(GameObject leader)
    {
        if (players.Contains(leader))
        {
            currentLeader = leader;
            FindObjectOfType<RaceHUDManager>().UpdateLeader(players.IndexOf(leader));
        }
        else
        {
            Debug.LogError("Attempting to set a non-player object as leader: " + leader.name);  // This seems like a mild error to throw given the circumstances
            return;
        }
    }

    private Material GetMaterialForColor(Color color)
    {
        if (color == Color.red) return redMaterial;
        if (color == Color.blue) return blueMaterial;
        if (color == Color.green) return greenMaterial;
        if (color == Color.yellow) return yellowMaterial;
        return null; // fallback
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

    public void NextLap(string pigName)
    {
        if (pigName != currentLeader.name)
        {
            Debug.LogWarning("Pig " + pigName + " is not the current leader, cannot advance lap.");
            return;
        }
        Spawner spawnerScript;
        currentLap++;
        if (currentLap >= numLaps)
        {
            // End the thing
        }
        else if (currentLap > 0)
        {
            // Note > 0 so we don't spawn objects on the first lap, done in Spawner.Start()
            foreach (GameObject spawner in spawners)
            {
                spawnerScript = spawner.GetComponent<Spawner>();
                spawnerScript.NewLapCleanup();
                spawnerScript.Spawn();
            }
        }
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