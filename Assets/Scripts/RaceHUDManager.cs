using UnityEngine;
using System.Collections.Generic;
public class RaceHUDManager : MonoBehaviour
{
    public PlayerHud[] playerHUDs;

    private List<GameObject> players;
    private RaceManager raceManager;
    void Start()
    {
        if (raceManager == null) { 
            raceManager = GameObject.FindGameObjectsWithTag("RaceManager")[0].GetComponent<RaceManager>();
        }

        players= raceManager.players;

        for (int i = 0; i < players.Count; i++) { 
            var curHud=playerHUDs[i];
            curHud.SetPlayerName(i+1);
            curHud.gameObject.SetActive(true);
        }
    }

    public void UpdateTurbo(int playerIndex, int turboCount)
    {
        playerHUDs[playerIndex].SetTurboCount(turboCount);
    }

    public void UpdateLeader(int leaderIndex)
    {
        for (int i = 0; i < playerHUDs.Length; i++)
        {
            playerHUDs[i].SetCrownVisible(i == leaderIndex);
        }
    }

    void Update()
    {
        
    }
}
