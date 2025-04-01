using UnityEngine;
using System.Collections.Generic;
public class RaceHUDManager : MonoBehaviour
{
    public PlayerHud[] playerHUDs;

    private List<GameObject> players;
    private RaceManager raceManager;
    void Start()
    {
        Debug.Log("HUDMGR STARTED");
        if (raceManager == null) { 
            raceManager = GameObject.FindGameObjectsWithTag("RaceManager")[0].GetComponent<RaceManager>();
        }

        players= raceManager.players;
        
    }

    public void SetupHUD(List<GameObject> players) {
        Debug.Log(" COUNT " + players.Count);
        var playerDataList = JoinManager.instance.playerDataList;
        for (int i = 0; i < players.Count; i++)
        {

            var curHud = playerHUDs[i];
            curHud.SetPlayerName(i + 1);
            curHud.SetPlayerColor(playerDataList[i].color);
            curHud.gameObject.SetActive(true);
            curHud.SetTurboCount(2);
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
