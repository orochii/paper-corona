using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextScreen : MonoBehaviour {
    [SerializeField] bool destroyPastData;

    public void LoadScreen(string name) {
        // Destroy previous saved data
        if (destroyPastData) {
            if (GameState.Instance != null) {
                Destroy(GameState.Instance.gameObject);
            }
        }
        // Load scene
        SceneLoader.Load(name);
    }
}
