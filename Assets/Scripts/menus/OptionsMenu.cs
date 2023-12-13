using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Dropdown resolutionDropdown;

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider gameplayVolumeSlider;

    private Resolution[] resolutions;

    public const string MIXER_MASTER = "MasterMixerVolume";
    public const string MIXER_MUSIC = "MusicVolume";
    public const string MIXER_SFX_GAMEPLAY = "SfxGameplayVolume";

    public static bool isInOptionsMenu = false;

    void Start()
    {
        resolutions = Screen.resolutions;

        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionOptions.Add(resolutions[i].width + "x" + resolutions[i].height);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();


        masterVolumeSlider.value = PlayerPrefs.GetFloat(SoundManager.MASTER_KEY, 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat(SoundManager.MUSIC_KEY, 1f);
        gameplayVolumeSlider.value = PlayerPrefs.GetFloat(SoundManager.SFX_GAMEPLAY_KEY, 1f);

        gameObject.SetActive(false);
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (isInOptionsMenu)  
            {
                isInOptionsMenu = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void EnteredOptionsMenu()
    {
        isInOptionsMenu = true;
    }
    
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat(MIXER_MASTER, Mathf.Log10(volume) * 20);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(MIXER_MUSIC, Mathf.Log10(volume) * 20);
    }
    public void SetGameplayVolume(float volume)
    {
        audioMixer.SetFloat(MIXER_SFX_GAMEPLAY, Mathf.Log10(volume) * 20);
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat(SoundManager.MASTER_KEY, masterVolumeSlider.value);
        PlayerPrefs.SetFloat(SoundManager.MUSIC_KEY, musicVolumeSlider.value);
        PlayerPrefs.SetFloat(SoundManager.SFX_GAMEPLAY_KEY, gameplayVolumeSlider.value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
    }
}
