using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryList : MonoBehaviour {
    [SerializeField] private Transform[] slots;
    [SerializeField] private InventoryElement elementPrefab;

    private List<InventoryElement> inventoryElements = new List<InventoryElement>();

    private void OnEnable() {
        GameState.Instance.InvOpen = true;
        // Destroy old objects
        if (inventoryElements.Count > 0) {
            foreach(InventoryElement ie in inventoryElements) {
                Destroy(ie.gameObject);
            }
            inventoryElements.Clear();
        }
        // Create new objects
        List<GameState.InvEntry> entries = GameState.Instance.ItemList;
        for (int i = 0; i < entries.Count && i < slots.Length; i++) {
            InventoryElement ie = Instantiate<InventoryElement>(elementPrefab, slots[i]);
            ie.Setup(entries[i]);
            ie.transform.localPosition = Vector3.zero;
            inventoryElements.Add(ie);
        }
    }

    private void OnDisable() {
        GameState.Instance.InvOpen = false;
    }
}