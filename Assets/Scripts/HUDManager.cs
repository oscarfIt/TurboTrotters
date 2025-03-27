using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }
    public List<string> playerNames = new List<string> { "Red", "Blue", "Green", "Yellow" };
    // public List<string> playerPositions = new List<string> { "Red", "Blue", "Green", "Yellow" };

    // [SerializeField] private TextMeshProUGUI[] playerPositionTexts;
    [SerializeField] private TextMeshProUGUI[] playerNameTexts;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetPlayerNames(playerNames);
    }

    // Set Player Names at the Start
    public void SetPlayerNames(List<string> playerNames)
    {
        int count = playerNames.Count;
        Debug.Log("Player Count: " + count);
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (i < count)
            {
                playerNameTexts[i].text = playerNames[i];
            }
            else
            {
                playerNameTexts[i].text = "";
            }
        }
    }

    // // Update Player Positions
    // public void UpdatePlayerPositions(string[] positions)
    // {
    //     for (int i = 0; i < playerPositionTexts.Length; i++)
    //     {
    //         if (i < positions.Length)
    //         {
    //             playerPositionTexts[i].text = positions[i];
    //         }
    //         else
    //         {
    //             playerPositionTexts[i].text = "";
    //         }
    //     }
    // }
}
