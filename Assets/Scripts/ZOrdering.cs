using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZOrdering : MonoBehaviour {
    void Start() {
        UpdateZOrder();
    }

    private void UpdateZOrder() {
        Vector3 pos = transform.position;
        pos.z = transform.position.y * .1f;
        transform.position = pos;
    }

    void Update() {
        UpdateZOrder();
    }
}