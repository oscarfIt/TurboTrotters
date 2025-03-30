using UnityEngine;
using UnityEngine.EventSystems;
public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject playButton;
    // public GameObject mapSelectPanel;

    public void OnPlayButton()
    {
        mainMenuPanel.SetActive(false);
      //  mapSelectPanel.SetActive(true);
        // Additional logic for initializing player joining can be placed here.
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
}
