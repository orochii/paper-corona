using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {
    public static GameState Instance { get; private set; }

    [System.Serializable]
    public class State {
        public float time = 0;
        public float hunger = 0;
        public List<InvEntry> inventory = new List<InvEntry>();
    }

    [System.Serializable]
    public class InvEntry {
        public int id;
        public int amount;

        public InvEntry(int id, int amount=0) {
            this.id = id;
            this.amount = amount;
        }
    }

    public static bool CallResult;

    [SerializeField] State state;
    [SerializeField] InvItem[] itemDatabase;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] UnityEngine.UI.Text timeLabel;
    [SerializeField] float baseHungerChange = 0.001f;
    [SerializeField] float multHungerChange = 0.1f;

    public void AddItem(int id, int amount) {
        InvEntry e = GetItemEntry(id);
        if (e == null) {
            if (amount < 0) return;
            e = new InvEntry(id, amount);
            state.inventory.Add(e);
        } else {
            e.amount += amount;
            if (e.amount <= 0) {
                state.inventory.Remove(e);
            }
        }
    }
    
    public List<InvEntry> ItemList { get {
            return state.inventory;
        } }

    public bool UIOpen {
        get {
            return InvOpen || SaveOpen || MinigameOpen || GameOver;
        }
    }

    public bool GameOver;
    public bool MinigameOpen;
    public bool InvOpen;
    public bool SaveOpen;

    public InvEntry GetItemEntry(int id) {
        foreach(InvEntry e in state.inventory) {
            if (e.id == id) return e;
        }
        return null;
    }

    public int GetItemAmount(int id) {
        InvEntry e = GetItemEntry(id);
        if (e == null) return 0;
        return e.amount;
    }

    public InvItem GetItem(int id) {
        return itemDatabase[id];
    }

    // 

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        // Stop hunger/time if UI is open.
        if (UIOpen) return;
        // Update time
        state.time += Time.deltaTime;
        int secs = (int)state.time % 60;
        int mins = (int)(state.time / 60) % 60;
        int hour = (int)(state.time / 3600) % 24;
        timeLabel.text = string.Format("{0:00}:{1:00}:{2:00}", hour, mins, secs);
        // Raise hungii :)
        state.hunger += UnityEngine.Random.Range(0, Time.deltaTime * (baseHungerChange + state.hunger * multHungerChange));
        spriteRenderer.color = Color.Lerp(Color.white, Color.blue, state.hunger/2);
        // Game Over state
        if (state.hunger > 1) {
            PlayerInteraction.Instance.PlayAnimation("PlayerDeath");
            StartCoroutine(AnimateGameOver());
            GameOver = true;
        }
    }

    public void Eat() {
        state.hunger = 0;
    }

    private IEnumerator AnimateGameOver() {
        CanvasGroup gameOver = MessageBox.Instance.GameOver;
        if (gameOver != null) {
            gameOver.gameObject.SetActive(true);
            gameOver.interactable = true;
            float perc = 0;
            while (perc < 1) {
                gameOver.alpha = perc;
                perc += Time.unscaledDeltaTime * 0.1f;
                yield return null;
            }
            gameOver.alpha = 1;
        }
        SceneLoader.Restart();
    }
}