using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
public class LobbyPlayer : MonoBehaviour
{
    public TMP_Text text;
    public Image border;
    public Image icon;
    public GameObject readyTick;

    public Color[] availableColors = new Color[] { Color.white, Color.blue, Color.green, Color.yellow };
    private int currentColorIndex = 0;

    public int playerIndex;

    public bool readyCheck;

    public AudioClip readySound;
    public AudioSource audioSource;
    public AudioClip unreadySound;
    void Awake()
    {
        
        readyTick.SetActive(false);
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerIndex = playerInput.playerIndex;
            
        }
    }
    void Start()
    {
        //  text = GetComponent<TextMeshProUGUI>();
     
        text.text = "Player " + (playerIndex+1);
        UpdateColor();

        readyCheck = false;
        UpdateReadyUI();

        // Notify the LobbyManager that this player has joined
        if (JoinManager.instance != null)
        {
            JoinManager.instance.OnPlayerJoined(this);
      
        }
    }

 
    public void OnCycleLeft()
    {
            CycleColorLeft();
    }

    public void OnCycleRight( )
    {
            CycleColorRight();
    }

    public void OnReady() { 
        toggleReady();
    }

    public void CycleColorRight()//cycle up
    {
        currentColorIndex = (currentColorIndex + 1) % availableColors.Length;
        UpdateColor();
    }
    public void CycleColorLeft()//cycle down
    {
        currentColorIndex = (currentColorIndex - 1 + availableColors.Length) % availableColors.Length;
        UpdateColor();
    }


    private void UpdateColor()
    {
       // text.color = availableColors[currentColorIndex];
       // border.color = availableColors[currentColorIndex];
        icon.color= availableColors[currentColorIndex];
       
    }

    // Optionally expose the chosen color
    public Color GetSelectedColor()
    {
        return availableColors[currentColorIndex];
    }

    public void toggleReady() {
        readyCheck = !readyCheck;

        if (readyCheck == true)
        {
            audioSource.PlayOneShot(readySound);
        }
        else { audioSource.PlayOneShot(unreadySound);
        }

        UpdateReadyUI();

        if (JoinManager.instance!=null) { 
            JoinManager.instance.OnPlayerReadyStateChanged(this);
        }
    }

    private void UpdateReadyUI() {
        if (readyCheck == true) {
            readyTick.SetActive(true);
        }
        else { readyTick.SetActive(false);}
    }



}

