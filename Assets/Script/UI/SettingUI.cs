using DG.Tweening;
using UnityEngine;

public class SettingUI : MonoBehaviour
{
    [SerializeField] DisplaySettings displaySettings;
    [SerializeField] AudioSettings audioSettings;

    [SerializeField] CanvasGroup canvasGroup;
    GameObject panel;
    [SerializeField] float fadeDuration = 0.25f;
    private void Awake()
    {
        panel = canvasGroup.transform.GetChild(0).gameObject;
        canvasGroup.alpha = 0;
        panel.SetActive(false);
    }

    public void CloseSettings()
    {
        displaySettings.ModifyData();
        audioSettings.ModifyData();
        SettingManager.Instance.SaveSettings();

        canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
        {
            panel.SetActive(false);
        });
    }

    public void OpenSettings()
    {
        panel.SetActive(true);
        canvasGroup.DOFade(1, fadeDuration);
    }
}
