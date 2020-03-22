using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Create inventory item")]
public class InvItem : ScriptableObject {
    public string displayName = "";
    public string description = "";
    public Sprite displayImage;
    public int actionId;
}