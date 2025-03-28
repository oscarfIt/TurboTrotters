using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; set; }
    public List<string> playerNames = new List<string> { "Red", "Blue", "Green", "Yellow" };
    public List<string> playerPositions = new List<string> { "", "", "", "" };

    // [SerializeField] private TextMeshProUGUI[] playerPositionTexts;
    [SerializeField] private TextMeshProUGUI[] playerNameTexts;
    [SerializeField] private TextMeshProUGUI[] playerPositionsTexts;

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
        SetPlayerPositions(playerPositions);

        UpdateHUDPosition(3, 4);
    }

    // Set Player Names at the Start
    public void SetPlayerNames(List<string> playerNames)
    {
        int count = playerPositions.Count;
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

    public void SetPlayerPositions(List<string> playerPositions)
    {
        int count = playerNames.Count;
        Debug.Log("Player Count: " + count);
        for (int i = 0; i < playerPositionsTexts.Length; i++)
        {
            if (i < count)
            {
                playerPositionsTexts[i].text = playerPositions[i];
            }
            else
            {
                playerPositionsTexts[i].text = "";
            }
        }
    }

    public void UpdateHUDPosition(int playerNumber, int position)
    {
        if (position == 1) { playerPositionsTexts[playerNumber].text = "1st"; }
        else if (position == 2) { playerPositionsTexts[playerNumber].text = "2nd"; }
        else if (position == 3) { playerPositionsTexts[playerNumber].text = "3rd"; }
        else if (position == 4) { playerPositionsTexts[playerNumber].text = "4th"; }
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
