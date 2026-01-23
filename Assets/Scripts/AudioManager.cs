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
            Debug.LogError( "No audio mixer found in the audio manager, sound settings will not work");
            Debug.Break();
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

        if (MuteMaster) audioMixer.SetFloat("Volume-Master", RangeToDb(0.00001f));
        else audioMixer.SetFloat("Volume-Master", RangeToDb(MasterAudioLevel));

        OnMasterAudioMuteChanged.Invoke(MuteMaster);
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

        if (MuteMaster) return;

        audioMixer.SetFloat("Volume-Master", RangeToDb(MasterAudioLevel));
        OnMasterAudioLevelChanged.Invoke(MasterAudioLevel);
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
        audioMixer.SetFloat("Volume-Music", RangeToDb(MusicAudioLevel));
        OnMusicAudioLevelChanged.Invoke(MusicAudioLevel);
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
        audioMixer.SetFloat("Volume-SFX", RangeToDb(SFXAudioLevel));
        OnSFXAudioLevelChanged.Invoke(SFXAudioLevel);
    }

    // Helper methods
    public float RangeToDb(float sliderValue)
    {
        return Mathf.Log10(sliderValue) * 20;
    }

}


