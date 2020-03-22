using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SceneLoader : MonoBehaviour {
    private static SceneLoader Instance;

    [SerializeField] Image fader;
    [SerializeField] Color fadeInColor = Color.white;
    [SerializeField] private string firstSceneName = "Title";
    private string currentSceneName;
    
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        fader.gameObject.SetActive(false);
        fader.color = Color.clear;
    }

    public static void Load(string newScene) {
        if (Instance == null) {
            SceneManager.LoadScene(newScene);
            return;
        }
        Instance.LoadScene(newScene);
    }

    public static void Restart() {
        Instance.LoadScene(Instance.firstSceneName);
    }

    public static void Reload() {
        Instance.LoadScene(Instance.currentSceneName);
    }

    Coroutine loadingSceneCoroutine;
    private void LoadScene(string newScene) {
        if (loadingSceneCoroutine != null) StopCoroutine(loadingSceneCoroutine);
        loadingSceneCoroutine = StartCoroutine(ExecuteSceneLoad(newScene));
    }

    private IEnumerator ExecuteSceneLoad(string newScene) {
        // Raise the fader
        fader.gameObject.SetActive(true);
        float perc = 0;
        while (perc < 1) {
            fader.color = fadeInColor * perc;
            perc += Time.unscaledDeltaTime;
            yield return null;
        }
        fader.color = Color.white;
        // Load the scene
        AsyncOperation oper = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
        while (!oper.isDone) {
            // Wait while the scene is loading
            yield return null;
        }
        currentSceneName = newScene;
        // Raise the fader
        perc = 0;
        while (perc < 1) {
            fader.color = fadeInColor * (1 - perc);
            perc += Time.unscaledDeltaTime;
            yield return null;
        }
        fader.color = Color.clear;
        fader.gameObject.SetActive(false);
    }
}