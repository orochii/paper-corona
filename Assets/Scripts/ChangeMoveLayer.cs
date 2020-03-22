using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMoveLayer : MonoBehaviour
{
    [SerializeField] int targetSortingLayer;
    [SerializeField] Collider2D deactivateCollider;
    [SerializeField] Collider2D activateCollider;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.isTrigger) return;
        PlayerMove pm = collision.GetComponent<PlayerMove>();
        if (pm != null) {
            pm.SetSortingLayer(targetSortingLayer);
            deactivateCollider.enabled = false;
            activateCollider.enabled = true;
        }
    }
}
