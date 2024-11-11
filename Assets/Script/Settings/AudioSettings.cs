using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] Slider masterSlider;
    [SerializeField] TMP_Text masterValue;
    [SerializeField] Slider musicSlider;
    [SerializeField] TMP_Text musicValue;
    [SerializeField] Slider sfxSlider;
    [SerializeField] TMP_Text sfxValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSettingData data = SettingManager.Instance.Data.Audio;

        masterSlider.value = data.MasterVolume;
        masterValue.text = (data.MasterVolume * 100).ToString("0");

        musicSlider.value = data.MusicVolume;
        musicValue.text = (data.MusicVolume * 100).ToString("0");

        sfxSlider.value = data.SfxVolume;
        sfxValue.text = (data.SfxVolume * 100).ToString("0");
    }

    void SetVolume(string parameter, Slider volumeSlider, TMP_Text text)
    {
        float volume = volumeSlider.value;
        text.text = (volume * 100).ToString("0"); //Max 1
        SettingManager.Instance.ActiveAudioMixer.SetFloat(parameter, Mathf.Log10(volume) * 20);
    }

    public void SetMasterVolume()
    {
        SetVolume("MasterVolume", masterSlider, masterValue);
    }
    public void SetMusicVolume()
    {
        SetVolume("MusicVolume", musicSlider, musicValue);
    }
    public void SetSfxVolume()
    {
        SetVolume("SfxVolume", sfxSlider, sfxValue);
    }

    public void ModifyData()
    {
        SettingManager.Instance.Data.Audio.MasterVolume = masterSlider.value;
        SettingManager.Instance.Data.Audio.MusicVolume = musicSlider.value;
        SettingManager.Instance.Data.Audio.SfxVolume = sfxSlider.value;
    }
}
