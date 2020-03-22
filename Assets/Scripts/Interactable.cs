using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InteractionData {
    public int kind;
    public Interaction interaction;
}

public class Interactable : MonoBehaviour {
    public static bool Busy;

    [SerializeField] private string displayDesc;
    public string Desc { get { return displayDesc; } }

    [SerializeField] private InteractionData[] interactions;

    private void OnMouseEnter() {
        if (GameState.Instance.UIOpen) return;
        PlayerInteraction.Instance.SetCurrent(this);
    }

    private void OnMouseExit() {
        if (GameState.Instance.UIOpen) return;
        PlayerInteraction.Instance.UnsetCurrent(this);
    }

    private void OnMouseUpAsButton() {
        if (Busy) return;
        if (GameState.Instance.UIOpen) return;
        PlayerInteraction.Instance.Interact(this);
    }

    Coroutine currentEvent;

    public void Execute(int kind) {
        if (Busy) return;
        // Find a proper interaction
        Interaction interaction = null;
        foreach (InteractionData i in interactions) if (i.kind == kind) interaction = i.interaction;
        if (interaction != null) {
            PlayerInteraction.Instance.UnsetCurrent(this);
            currentEvent = StartCoroutine(interaction.ExecuteCommandList(interaction.commands));
        } else {
            // Use a vanilla interaction.
            Interaction vanillaInteraction = PlayerInteraction.Instance.GetVanillaInteraction();
            currentEvent = StartCoroutine(vanillaInteraction.ExecuteCommandList(vanillaInteraction.commands));
        }
    }

    internal void UseItem(InvItem currentItem) {
        Execute(currentItem.actionId);
    }

    public void Deactivate() {
        gameObject.SetActive(false);
        Busy = false;
    }
}