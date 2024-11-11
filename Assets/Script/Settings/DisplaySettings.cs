using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySettings : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle fullScreenToggle;
    [SerializeField] Toggle vSyncToggle;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private float currentRefreshRate;
    private int currentResolutionIndex = 0;
    private void Awake()
    {
        SetupResolutionDropdown();
        SetupFullScreen();
        SetupVSync();
    }

    void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        filteredResolutions = new List<Resolution>();
        resolutions = Screen.resolutions;

        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if ((float)resolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        filteredResolutions.Sort((a, b) =>
        {
            if (a.width != b.width)
                return b.width.CompareTo(a.width);
            else
                return b.height.CompareTo(a.height);
        });

        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            // string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRateRatio.value.ToString("0") + " Hz";
            string resolutionOption = ResolutionHelper.ResolutionToString(filteredResolutions[i]);
            options.Add(resolutionOption);
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height && (float)filteredResolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    void SetupFullScreen()
    {
        fullScreenToggle.isOn = Screen.fullScreen;
        fullScreenToggle.onValueChanged.AddListener(OnFullScreenToggleValueChanged);
    }

    void SetupVSync()
    {
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
        vSyncToggle.onValueChanged.AddListener(OnVSyncToggleValueChanged);
    }

    private void OnFullScreenToggleValueChanged(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    private void OnVSyncToggleValueChanged(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ModifyData()
    {
        SettingManager.Instance.Data.Display.Resolution = ResolutionHelper.ResolutionToString(filteredResolutions[resolutionDropdown.value]);
        SettingManager.Instance.Data.Display.FullScreen = fullScreenToggle.isOn;
        SettingManager.Instance.Data.Display.VSync = vSyncToggle.isOn;
    }

}
