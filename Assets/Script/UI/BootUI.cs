using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BootUI : MonoBehaviour
{
    public Slider ProgressBar;
    public TMP_Text BootText;
    public string BootCompleteMessage = "Initialization Complete!";
    private int totalInitializable;
    private int initialized = 0;

    void Start()
    {
        totalInitializable = Boot.Instance.Initializables.Count;
        ProgressBar.value = 0;
        BootText.text = "";
        StartCoroutine(ProgressReader());
    }

    IEnumerator ProgressReader()
    {
        foreach (var mono in Boot.Instance.Initializables)
        {
            if (mono is IInitializable initializable)
            {
                BootText.text = initializable.InitializationDisplayText;

                // Wait until this specific initializable completes its initialization
                yield return new WaitUntil(() => initializable.IsInitialized);

                initialized++;
                ProgressBar.value = (float)initialized / totalInitializable;
            }
            else
            {
                Debug.LogWarning($"{mono.name} does not implement IInitializable and will be skipped.");
            }
        }

        // Finalize progress bar
        BootText.text = BootCompleteMessage;
        ProgressBar.value = 1f;
    }
}
