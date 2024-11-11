using System;
using UnityEngine;

[Serializable]
public class SettingData
{
    public AudioSettingData Audio = new AudioSettingData();
    public DisplaySettingData Display = new DisplaySettingData();
}

[Serializable]
public class AudioSettingData
{
    public float MasterVolume = 1;
    public float MusicVolume = 1;
    public float SfxVolume = 1;
}

[Serializable]
public class DisplaySettingData
{
    public string Resolution;
    public bool FullScreen;
    public bool VSync;
}