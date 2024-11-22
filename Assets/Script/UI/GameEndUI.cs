using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NaughtyAttributes;

public class GameEndUI : MonoBehaviour
{
    [Header("Components")]
    public Image Panel;
    public Image StatusMessage;
    public Button MainMenuButton;

    [Header("Messages")]
    public List<Sprite> WinMessage;
    public List<Sprite> LoseMessage;
    public List<Sprite> TimeUpMessage;

    [Header("Colors")]
    public Color WinMessageStartColor = Color.yellow;
    public Color LoseMessageStartColor = Color.red;
    public Color TimeUpMessageStartColor = Color.magenta;
    public Color MessageFinalColor = Color.white;

    [Header("Animation Settings")]
    public float PanelFadeAlpha = 0.4f;
    public float PanelFadeDuration = 1f;
    public float MessageFadeDuration = 1f;
    public float ButtonShowDelay = 0.5f;

    [Button]
    public void PlayLoseAnimation()
    {
        PlayAnimation(LoseMessage, LoseMessageStartColor);
    }

    [Button]
    public void PlayWinAnimation()
    {
        PlayAnimation(WinMessage, WinMessageStartColor);
    }

    [Button]
    public void PlayTimeUpAnimation()
    {
        PlayAnimation(TimeUpMessage, TimeUpMessageStartColor);
    }

    private void PlayAnimation(List<Sprite> messageList, Color startColor)
    {
        Panel.gameObject.SetActive(true);
        StatusMessage.gameObject.SetActive(false);
        MainMenuButton.gameObject.SetActive(false);

        StatusMessage.sprite = messageList[Random.Range(0, messageList.Count)];
        StatusMessage.color = startColor;

        Panel.color = new Color(Panel.color.r, Panel.color.g, Panel.color.b, 0);
        Panel.DOFade(PanelFadeAlpha, PanelFadeDuration)
             .SetUpdate(true)
             .OnComplete(() =>
             {
                 
                 StatusMessage.gameObject.SetActive(true);
                 StatusMessage.DOFade(1, MessageFadeDuration)
                              .SetUpdate(true)
                              .OnComplete(() =>
                              {
                                  StatusMessage.DOColor(MessageFinalColor, MessageFadeDuration)
                                               .SetUpdate(true)
                                               .OnComplete(() =>
                                               {
                                                   
                                                   DOVirtual.DelayedCall(ButtonShowDelay, () =>
                                                   {
                                                       MainMenuButton.gameObject.SetActive(true);
                                                   }).SetUpdate(true);
                                               });
                              });
             });
    }
}
