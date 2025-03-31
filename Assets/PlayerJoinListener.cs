using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinListener : MonoBehaviour
{
    private void Start()
    {
        // Get the PlayerInputManager component and subscribe to the event
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();
        if (playerInputManager != null)
        {
            playerInputManager.onPlayerJoined += OnPlayerJoined;
        }
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log($"Player {playerInput.playerIndex} joined!");

        // Example: Send event to update HUD
        // HUDManager.Instance.UpdateHUD();
    }
}
