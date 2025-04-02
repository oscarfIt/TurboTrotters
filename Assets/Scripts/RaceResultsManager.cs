using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class RaceResultsManager : MonoBehaviour
{
    public GameObject resultsPanel;
    public TextMeshProUGUI resultsText;
    public GameObject returnButton;

    public void ShowResults(int winningPlayerIndex)
    {
        resultsPanel.SetActive(true);
        Time.timeScale = 0f;

        resultsText.text = $"<b><color=#FFD700>Player {winningPlayerIndex + 1} Wins!</color></b>";
        EventSystem.current.SetSelectedGameObject(returnButton);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // update with your scene name
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
