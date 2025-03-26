using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<string> playerNames = new List<string>(); // Stores player names

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

    public void StartGame()
    {
        SceneManager.LoadScene("CharacterSelection"); // Go to Character Selection
    }

    public void StartRace()
    {
        SceneManager.LoadScene("Map_1"); // Load the race track
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
