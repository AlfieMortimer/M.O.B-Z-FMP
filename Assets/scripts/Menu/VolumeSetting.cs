using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    
    
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider masSlider;
    
    bool isMuted;
    
    public static VolumeSetting instance;

/*
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
*/
    public void muteToggle(Toggle muted)
    {
        
        if ( muted.isOn)
        {
            myMixer.SetFloat("Music", -80);
            isMuted = true;
        }
        else
        {
            LoadVolume();
            isMuted = false;
            SetMusicVolume();
        }
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("masVolume"))
        {
            LoadVolume1();
        }
        else
        {
            SetMasVolume();
        }
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetSfxVolume();
        }
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
        }


        SetMusicVolume();
    }
    public void SetMasVolume()
    {
        float volume = masSlider.value;
        myMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("masVolume", volume);
    }



    public void SetSfxVolume()
    {
        float volume = sfxSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
    public void SetMusicVolume()
    {
        if (isMuted == false)
        {
            float volume = musicSlider.value;
            myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("musicVolume", volume);
        }
        
    }
    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetSfxVolume();

    }
    private void LoadVolume1()
    {
        masSlider.value = PlayerPrefs.GetFloat("masVolume"); 
        SetMasVolume();
        
    }
    
}
