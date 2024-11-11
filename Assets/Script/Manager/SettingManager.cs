using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class SettingManager : MonoBehaviour, IInitializable
{
    public static string SettingPath { get; private set; }
    public static SettingManager Instance;

    public SettingData Data;
    public AudioMixer ActiveAudioMixer;
    [field: SerializeField] public string InitializationDisplayText { get; private set; } = "Initializing Settings";
    public bool IsInitialized { get; private set; } = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(transform.parent == null ? gameObject : this);
            return;
        }
        Instance = this;
        if (transform.parent == null) DontDestroyOnLoad(gameObject);

        SettingPath = Path.Combine(Application.persistentDataPath, "setting.json");
    }

    private void Start()
    {
        if (Boot.Instance == null) Init();
    }

    private void SetTargetFrameRate()
    {
        int screenRefreshRate = Convert.ToInt32(Screen.currentResolution.refreshRateRatio.value);
        Application.targetFrameRate = screenRefreshRate;
    }

    public IEnumerator Init()
    {
        if (File.Exists(SettingPath))
        {
            LoadSettings();
        }
        else
        {
            InitializeDefaultSettings();
        }

        // Optionally set target frame rate or apply other settings after loading
        SetTargetFrameRate();

        IsInitialized = true;
        yield return null;
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(SettingPath, json);
        Debug.Log("Settings saved to " + SettingPath);
    }

    public void LoadSettings()
    {
        if (File.Exists(SettingPath))
        {
            try
            {
                string json = File.ReadAllText(SettingPath);
                Data = JsonUtility.FromJson<SettingData>(json);
                Debug.Log("Settings loaded from " + SettingPath);
                ApplyLoadedSettings();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load settings from {SettingPath}: {ex.Message}");
                InitializeDefaultSettings();
            }
        }
        else
        {
            Debug.LogWarning("Settings file not found at " + SettingPath);
            InitializeDefaultSettings();
        }
    }

    private void InitializeDefaultSettings()
    {
        Data = new SettingData();
        Data.Display.Resolution = Screen.currentResolution.ToString();
        Data.Display.FullScreen = Screen.fullScreen;
        Data.Display.VSync = (QualitySettings.vSyncCount > 0);
        SaveSettings();
        Debug.Log("Initialized default settings.");
    }

    private void ApplyLoadedSettings()
    {
        // Set Resolution
        if (ResolutionHelper.TryParseResolution(Data.Display.Resolution, out int resolutionWidth, out int resolutionHeight))
        {
            Screen.SetResolution(resolutionWidth, resolutionHeight, Data.Display.FullScreen);
        }
        else
        {
            //Replace faulty resolution setting
            Data.Display.Resolution = ResolutionHelper.ResolutionToString(Screen.currentResolution);
            SaveSettings();
        }

        // Set VSync
        QualitySettings.vSyncCount = Data.Display.VSync ? 1 : 0;

        //Set Audio
        ActiveAudioMixer.SetFloat("MasterVolume", Mathf.Log10(Data.Audio.MasterVolume) * 20);
        ActiveAudioMixer.SetFloat("MusicVolume", Mathf.Log10(Data.Audio.MusicVolume) * 20);
        ActiveAudioMixer.SetFloat("SfxVolume", Mathf.Log10(Data.Audio.SfxVolume) * 20);

        Debug.Log("Applied loaded settings.");
    }
}