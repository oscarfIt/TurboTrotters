using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerHud : MonoBehaviour
{
    public TMP_Text playerNameText;
    public Image[] turboIcons;
    public Image crownIcon;


    void Start()
    {
       // playerNameText.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetPlayerName(int playerNum) {
        playerNameText.text = "Player " + playerNum;
        playerNameText.enabled = true;
        Debug.Log("NAMED");
    }
    public void SetTurboCount(int numTurbo)
    {
        for (int i = 0; i < turboIcons.Length; i++) { 
            turboIcons[i].enabled = (i<numTurbo);
        }
    }

    public void SetCrownVisible(bool isVisible)
    {
        crownIcon.enabled = isVisible;
    }

}
