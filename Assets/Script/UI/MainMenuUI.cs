using DG.Tweening;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public CanvasGroup CreditCanvasGroup;
    public GameObject CreditPanel;
    private float fadeDuration = 0.25f;

    public void OpenGameplay()
    {
        SceneLoader.Instance.LoadScene(2);
    }
    public void OpenMainMenu()
    {
        SceneLoader.Instance.LoadScene(1);
    }
    public void CloseCredit()
    {
        CreditCanvasGroup.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
                {
                    CreditPanel.SetActive(false);
                });
    }
    public void OpenCredit()
    {
        CreditPanel.SetActive(true);
        CreditCanvasGroup.DOFade(1, fadeDuration).SetUpdate(true);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}