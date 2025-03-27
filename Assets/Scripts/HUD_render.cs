using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD_render : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadScene("HUD", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
