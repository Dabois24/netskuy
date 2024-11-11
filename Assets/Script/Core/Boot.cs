using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boot : MonoBehaviour
{
    public static Boot Instance;
    public List<MonoBehaviour> Initializables;
    public UnityEvent OnBootComplete;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {

        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        foreach (MonoBehaviour mono in Initializables)
        {
            if (mono is IInitializable initializable)
            {
                yield return StartCoroutine(initializable.Init());
            }
            else
            {
                Debug.LogWarning($"{mono.name} does not implement IInitializable and will be skipped.");
            }
        }
        Debug.Log("Boot Completed!");
        OnBootComplete?.Invoke();

        // Unsubscribe all listeners after invocation
        OnBootComplete.RemoveAllListeners();
    }

    
}
