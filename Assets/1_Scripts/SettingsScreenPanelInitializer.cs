using UnityEngine;
using UnityEngine.UI;

public class SettingsScreenPanelInitializer : MonoBehaviour
{

    [SerializeField] AudioManager audioManager;
    [SerializeField] Toggle MuteCheckbox;
    [SerializeField] Slider MasterSlider;
    [SerializeField] Slider MusicSlider;
    [SerializeField] Slider SFXSlider;

    private void Start()
    {
        MuteCheckbox.isOn = audioManager.GetMuteMaster();
        MasterSlider.value = audioManager.GetMasterAudioLevel();
        MusicSlider.value = audioManager.GetMusicAudioLevel();
        SFXSlider.value = audioManager.GetSFXAudioLevel();

    }

}
