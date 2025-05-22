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
        if(masSlider.value > 0)
        {
            myMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("masVolume", volume);
        }
        else
        {
            myMixer.SetFloat("Master", -80f);
            PlayerPrefs.SetFloat("masVolume", volume);
        }
    }



    public void SetSfxVolume()
    {
        float volume = sfxSlider.value;
        if(sfxSlider.value > 0)
        {
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }
        else
        {
            myMixer.SetFloat("SFX", -80);
        }
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
    public void SetMusicVolume()
    {
        if (isMuted == false)
        {
            float volume = musicSlider.value;
            if (musicSlider.value > 0) 
            { 
                myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
            }
            else
            {
                myMixer.SetFloat("Music", -80);
            }
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
    public void QuitGame()
    {
        Application.Quit();
    }

}
