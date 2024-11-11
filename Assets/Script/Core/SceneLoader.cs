using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    [SerializeField] private GameObject loadingCurtain;
    [SerializeField] private Slider progressBar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneToLoad)
    {
        StartCoroutine(LoadSceneASync(sceneToLoad));
    }

    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
            LoadScene(scenePath);
        }
        else
        {
            Debug.LogWarning("Invalid scene index");
        }
    }

    private IEnumerator LoadSceneASync(string sceneToLoad)
    {
        loadingCurtain.SetActive(true);

        // Begin async load
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneToLoad);
        load.allowSceneActivation = false;

        // Update slider while loading progresses
        while (load.progress < 0.9f)
        {
            progressBar.value = Mathf.Clamp01(load.progress / 0.9f);
            yield return null;
        }

        // Simulate final progress to 1.0
        progressBar.value = 1f;
        yield return new WaitForSecondsRealtime(0.5f);

        // Activate scene once complete
        load.allowSceneActivation = true;
        yield return new WaitForSecondsRealtime(0.5f);

        // Deactivate curtain after load completes
        loadingCurtain.SetActive(false);
    }
}
