using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour {
    public static PlayerInteraction Instance { get; private set; }
    public bool HasCurrent { get { return current != null; } }
    public static InvItem CurrentItem;

    private Vector2 screenSize = new Vector3(1920, 1080);

    [SerializeField] PlayerMove playerMove;
    [SerializeField] Text currentObjLabel;
    [SerializeField] GameObject coinUp;
    [SerializeField] RectTransform coinUpWindow;
    [SerializeField] InventoryList inventory;
    [SerializeField] float marginHorz = 340;
    [SerializeField] float marginVert = 100;
    [SerializeField] Image currentItemIcon;
    [SerializeField] Interaction[] vanillaInteractions;
    private Interactable current;

    public PlayerMove Move {
        get {
            return playerMove;
        }
    }

    public void SetCurrent(Interactable obj) {
        if (coinUp.activeInHierarchy) return;
        current = obj;
        if (CurrentItem != null) {
            currentObjLabel.text = "Usar " + CurrentItem.displayName + " en " + obj.Desc;
        } else {
            currentObjLabel.text = "Interactuar con " + obj.Desc;
        }
    }
    public void UnsetCurrent(Interactable obj) {
        if (coinUp.activeInHierarchy) return;
        if (current == obj) {
            current = null;
            currentObjLabel.text = "";
        }
    }

    public void OpenInventory() {
        if (CurrentItem != null) {
            CurrentItem = null;
            return;
        }
        inventory.gameObject.SetActive(true);
    }

    internal Interaction GetVanillaInteraction() {
        int rndIdx = UnityEngine.Random.Range(0, vanillaInteractions.Length);
        return vanillaInteractions[rndIdx];
    }

    public void CloseCoinUp() {
        current = null;
        currentObjLabel.text = "";
        coinUp.SetActive(false);
    }

    internal void Interact(Interactable interactable) {
        // If you got an item
        if (CurrentItem != null) {
            interactable.UseItem(CurrentItem);
            CurrentItem = null;
            return;
        }
        // Setup coin-up
        Vector3 pos = Camera.main.WorldToViewportPoint(interactable.transform.position);
        pos.x = Mathf.Clamp(pos.x * screenSize.x, marginHorz, screenSize.x - marginHorz);
        pos.y = Mathf.Clamp(pos.y * screenSize.y, marginVert, screenSize.y - marginVert);
        coinUpWindow.anchoredPosition = pos;
        coinUp.SetActive(true);
    }

    public void ExecuteInteraction(int id) {
        if (current != null) {
            current.Execute(id);
        }
        CloseCoinUp();
    }

    void Awake() {
        Instance = this;
    }

    private void Update() {
        // Update current interaction when necesary
        if (current != null && !current.gameObject.activeInHierarchy) UnsetCurrent(current);
        currentObjLabel.gameObject.SetActive(!Interactable.Busy);
        // Update current item
        if (CurrentItem != null) {
            currentItemIcon.transform.position = Input.mousePosition;
            currentItemIcon.sprite = CurrentItem.displayImage;
            currentItemIcon.color = Color.white;
        } else {
            currentItemIcon.color = Color.clear;
        }
    }

    internal void PlayAnimation(string v) {
        playerMove.PlayAnimation(v);
    }
}
