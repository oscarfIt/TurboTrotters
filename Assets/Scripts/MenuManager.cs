using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        GameManager.Instance.StartGame();
    }

    public void StartRace()
    {
        GameManager.Instance.StartRace();
    }

    public void Return()
    {
        GameManager.Instance.GoHome();
    }

    public void CloseSettings()
    {
        SceneManager.UnloadSceneAsync("Settings");
    }

    public void OpenSettings()
    {
        GameManager.Instance.OpenSettings();
    }

    public void PauseGame()
    {
        GameManager.Instance.PauseGame();
    }

    public void UnpauseGame()
    {
        SceneManager.UnloadSceneAsync("PauseMenu");
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
