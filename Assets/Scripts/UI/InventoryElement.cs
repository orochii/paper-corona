using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryElement : MonoBehaviour {
    [SerializeField] Image iconImage;
    [SerializeField] Text amountLabel;

    InvItem invItem;

    public void Setup(GameState.InvEntry entry) {
        invItem = GameState.Instance.GetItem(entry.id);
        iconImage.sprite = invItem.displayImage;
        amountLabel.text = entry.amount.ToString();
    }

    public void OnClick() {
        Debug.Log("Usar " + invItem.displayName + " del inventario");
        PlayerInteraction.CurrentItem = invItem;
    }
}
