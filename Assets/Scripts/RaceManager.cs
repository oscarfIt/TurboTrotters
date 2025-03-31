using UnityEngine;
using UnityEngine.InputSystem;

public class RaceManager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        player.gameObject.name = "Player_" + player.playerIndex;
    }
}