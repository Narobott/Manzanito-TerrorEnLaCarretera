using System;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{

    public AudioManager()
    {
        MuteMaster = false;
        MasterAudioLevel = 1.0f;
        MusicAudioLevel = 1.0f;
        SFXAudioLevel = 1.0f;
    }

    private void Awake()
    {
        if (audioMixer == null)
        {
            Debug.LogError("No audio mixer found in the audio manager, sound settings will not work");
            Debug.Break();
        }
        if (PlayerPrefs.HasKey("MuteMaster"))
        {
            SetMuteMaster(PlayerPrefs.GetInt("MuteMaster") == 1 ? true : false);
        }
        if (PlayerPrefs.HasKey("MasterAudioLevel"))
        {
            SetMasterAudioLevel(PlayerPrefs.GetFloat("MasterAudioLevel"));
        }
        if (PlayerPrefs.HasKey("MusicAudioLevel"))
        {
            SetMusicAudioLevel(PlayerPrefs.GetFloat("MusicAudioLevel"));
        }
        if(PlayerPrefs.HasKey("SFXAudioLevel"))
        {
            SetSFXAudioLevel(PlayerPrefs.GetFloat("SFXAudioLevel"));
        }
    }

    // Audio mixer reference
    [SerializeField]
    AudioMixer audioMixer;

    // Mute master
    public UnityEvent<bool> OnMasterAudioMuteChanged;

    private bool MuteMaster;
    public void SetMuteMaster(bool mute)
    {
        MuteMaster = mute;


        PlayerPrefs.SetInt("MuteMaster", MuteMaster ? 1 : 0);

        if (MuteMaster) audioMixer.SetFloat("Volume-Master", RangeToDb(0.00001f));
        else audioMixer.SetFloat("Volume-Master", RangeToDb(MasterAudioLevel));

        OnMasterAudioMuteChanged.Invoke(MuteMaster);
    }

    public bool GetMuteMaster()
    {
        return MuteMaster;
    }

    // Master audio level
    public UnityEvent<float> OnMasterAudioLevelChanged;

    private float _masterAudioLevel;
    private float MasterAudioLevel
    {
        get { return _masterAudioLevel; }
        set { _masterAudioLevel = Mathf.Clamp(value, 0.0001f, 1.0f); }
    }

    public void SetMasterAudioLevel(float newMasterAudioLevel)
    {
        MasterAudioLevel = newMasterAudioLevel;


        PlayerPrefs.SetFloat("MasterAudioLevel", MasterAudioLevel);

        if (MuteMaster) return;

        audioMixer.SetFloat("Volume-Master", RangeToDb(MasterAudioLevel));
        OnMasterAudioLevelChanged.Invoke(MasterAudioLevel);
    }

    public float GetMasterAudioLevel()
    {
        return MasterAudioLevel;
    }


    // Music audio level
    public UnityEvent<float> OnMusicAudioLevelChanged;

    private float _musicAudioLevel;
    private float MusicAudioLevel
    {
        get { return _musicAudioLevel; }
        set { _musicAudioLevel = Mathf.Clamp(value, 0.0001f, 1.0f); }
    }

    public void SetMusicAudioLevel(float newMusicAudioLevel)
    {
        MusicAudioLevel = newMusicAudioLevel;

        PlayerPrefs.SetFloat("MusicAudioLevel", MusicAudioLevel);

        audioMixer.SetFloat("Volume-Music", RangeToDb(MusicAudioLevel));
        OnMusicAudioLevelChanged.Invoke(MusicAudioLevel);
    }

    public float GetMusicAudioLevel()
    {
        return MusicAudioLevel;
    }

    // SFX Audio Level

    public UnityEvent<float> OnSFXAudioLevelChanged;

    private float _sfxAudioLevel;
    private float SFXAudioLevel
    {
        get { return _sfxAudioLevel; }
        set { _sfxAudioLevel = Mathf.Clamp(value, 0.0001f, 1.0f); }
    }

    public void SetSFXAudioLevel(float newSFXAudioLevel)
    {
        SFXAudioLevel = newSFXAudioLevel;

        PlayerPrefs.SetFloat("SFXAudioLevel", SFXAudioLevel);

        audioMixer.SetFloat("Volume-SFX", RangeToDb(SFXAudioLevel));
        OnSFXAudioLevelChanged.Invoke(SFXAudioLevel);
    }

    public float GetSFXAudioLevel()
    {
        return SFXAudioLevel;
    }

    // Helper methods
    public float RangeToDb(float sliderValue)
    {
        return Mathf.Log10(sliderValue) * 20;
    }

}


