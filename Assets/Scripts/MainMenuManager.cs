using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject helpPanel;
    public GameObject playButton;
    public GameObject helpButton;
    public GameObject exitHelpButton;
    // public GameObject mapSelectPanel;

    public void OnPlayButton()
    {
        SceneManager.LoadScene("SelectionLobby");
    }

    // Called when the Options button is pressed
    public void OnOptionsButton()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    // Called when the Quit button is pressed
    public void OnQuitButton()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();
    }

    public void OnBackFromOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(playButton);
    }

    public void OnHelpButton() {
        mainMenuPanel.SetActive(false);
        helpPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(exitHelpButton);
    }
    public void OnExitHelpButton() {
        mainMenuPanel.SetActive(true);
        helpPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(playButton);
    }
}
