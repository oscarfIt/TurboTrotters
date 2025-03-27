using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int playerCount = 2; // Default 2 player
    private string selectedMap = "Map_1"; // Default map
    private List<string> availableMaps = new List<string> { "Map_1" };
    // public List<string> playerNames = new List<string> { "Red", "Blue", "Green", "Yellow" }; // Stores player names

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this GameManager across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // public List<string> GetPlayerNames()
    // {
    //     return playerNames;
    // }

    public int GetPlayerCount()
    {
        return playerCount;
    }
    public void SetPlayerCount(int count)
    {
        playerCount = Mathf.Clamp(count, 1, 4); // Ensure it's between 1 and 4
        Debug.Log("Player Count Set: " + playerCount);
    }

    public string GetSelectedMap()
    {
        return selectedMap;
    }

    public void SetSelectedMap(string mapName)
    {
        selectedMap = mapName;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("CharacterSelection"); // Go to Character Selection
    }

    public void StartRace()
    {
        SceneManager.LoadScene("Map_1"); // Load the race track
        // HUDManager.Instance.SetPlayerNames(playerNames);
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings", LoadSceneMode.Additive); // Load settings (Optional)
    }

    public void PauseGame()
    {
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive); // Load settings (Optional)
    }

    public void GoHome()
    {
        SceneManager.LoadScene("MainMenu"); // Load main menu
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
