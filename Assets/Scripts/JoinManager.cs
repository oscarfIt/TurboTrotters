using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class JoinManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static JoinManager instance;

    public bool allowJoining = true;

    public List<LobbyPlayer> joinedPlayers = new List<LobbyPlayer>();

    public GameObject joinPanel;
    public GameObject mapSelectPanel;

    public GameObject lobbyPlayerPrefab;
    public Transform playerListContainer;

    public GameObject map1Btn;

    public CanvasGroup joinPanelCanvasGroup;
    // Selected map name (default)
    public string selectedMap;

    private void Awake()
    {
        joinPanel.SetActive(true);
        
        mapSelectPanel.SetActive(false);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPlayerJoined(LobbyPlayer player)
    {
        if (!allowJoining) { return; }
        //  player.transform.parent = playerListContainer;
        if (joinPanel.activeInHierarchy==true) { 
        if (!joinedPlayers.Contains(player))
         {
            player.transform.SetParent(playerListContainer);
            joinedPlayers.Add(player);
             Debug.Log("Player joined. Total players: " + joinedPlayers.Count);

        }
        }

    }

    public void OnPlayerJoined(PlayerInput input) { }//needed to prevent runtime err- does nothing

    public void OnPlayerReadyStateChanged(LobbyPlayer player) {
        if (allPlayersReady()) { 
            ProceedToMapSelection();
        }
    }


    private bool allPlayersReady() {
        if (joinedPlayers.Count == 0)
            return false;
        foreach (LobbyPlayer player in joinedPlayers)
        {
            if (!player.readyCheck)
                return false;
        }
        return true;
    }
    public void ProceedToMapSelection()
    {
        // Hide the join panel and show the map selection panel.
 
        allowJoining = false;
        HideJoinPanel();
  
        mapSelectPanel.SetActive(true);
        Debug.Log("All players ready, Proceeding to map selection");

        foreach (LobbyPlayer player in joinedPlayers)
        {
   
            PlayerInput input = player.GetComponent<PlayerInput>();
            if (input != null)
            {
                input.SwitchCurrentActionMap("UI");
            }
        }

      EventSystem.current.SetSelectedGameObject(map1Btn);
    }

    public void SelectMap(string mapName)
    {
        selectedMap = mapName;
        Debug.Log("Map selected: " + mapName);
    }

    public void StartGame()
    {
        // Optionally, pass the player data to a Game Manager.
        // Then load the selected map scene.
        Debug.Log("Starting game on map: " + selectedMap + " with " + joinedPlayers.Count + " players.");
        SceneManager.LoadScene(selectedMap);
    }
    public void OnMap1Button()
    {
        SelectMap("Map_1");
    }

    public void HideJoinPanel()
    {
        if (joinPanelCanvasGroup != null)
        {
            joinPanelCanvasGroup.alpha = 0f;          // Make the panel invisible
            joinPanelCanvasGroup.interactable = false;  // Disable UI interaction
            joinPanelCanvasGroup.blocksRaycasts = false; // Allow clicks to pass through
        }
    }
}
