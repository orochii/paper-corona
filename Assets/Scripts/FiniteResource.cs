using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FiniteResource : MonoBehaviour {
    [SerializeField] Sprite[] stages;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] UnityEvent onDeplete;

    int currStage;
    bool depleted;
    
    public void Advance() {
        currStage++;
        if (currStage < stages.Length) {
            sprite.sprite = stages[currStage];
        } else {
            if (onDeplete != null) onDeplete.Invoke();
            depleted = true;
        }
    }

    private void Update() {
        if (depleted && !Interactable.Busy) gameObject.SetActive(false);
    }
}