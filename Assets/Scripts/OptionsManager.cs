using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
public class OptionsManager : MonoBehaviour
{
    public Slider masterVol, musicVol, sfxVol;
    public AudioMixer audioMixer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnEnable()
    {
        // Ensure the slider is selected when the options menu becomes active
        EventSystem.current.SetSelectedGameObject(masterVol.gameObject);
        
    }

    public void SetMasterVolume()
    {
        audioMixer.SetFloat("masterVolume", masterVol.value);
    }

    public void SetMusicVolume()
    {
        audioMixer.SetFloat("musicVolume",musicVol.value );
    }

    public void SetSFXVolume()
    {
        audioMixer.SetFloat("sfxVolume",sfxVol.value );
    }
}
