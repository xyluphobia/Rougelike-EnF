using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    public AudioSource sfxGameplaySource;
    public AudioSource sfxUISource;
    public AudioSource musicSource;
    public static SoundManager instance = null;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    public const string MASTER_KEY = "MasterMixerVolume";
    public const string MUSIC_KEY = "MusicVolume";
    public const string SFX_GAMEPLAY_KEY = "SfxGameplayVolume";

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance!= this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        LoadVolume();
    }

    public void PlaySound(AudioClip clip)
    {
        sfxGameplaySource.PlayOneShot(clip);
    }

    public void RandomizeSfx (params AudioClip[] clips)
    {
        if (clips.Length == 0) {return;}

        int randomindex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        // Debug.Log(randomPitch);

        sfxGameplaySource.pitch = randomPitch;
        sfxGameplaySource.PlayOneShot(clips[randomindex]);
    }

    void LoadVolume()   // Volume saved in OptionsMenu.cs
    {
        float masterVolume = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float gameplayVolume = PlayerPrefs.GetFloat(SFX_GAMEPLAY_KEY, 1f);

        // Debug.Log(masterVolume + " " + musicVolume + " " + gameplayVolume);

        audioMixer.SetFloat(OptionsMenu.MIXER_MASTER, Mathf.Log10(masterVolume) * 20);
        audioMixer.SetFloat(OptionsMenu.MIXER_MUSIC, Mathf.Log10(musicVolume) * 20);
        audioMixer.SetFloat(OptionsMenu.MIXER_SFX_GAMEPLAY, Mathf.Log10(gameplayVolume) * 20);
    }

    public void PauseGameplaySfx()
    {
        audioMixer.SetFloat(OptionsMenu.MIXER_SFX_GAMEPLAY, -80);
    }
    public void ResumeGameplaySfx()
    {
       LoadVolume();
    }
}
