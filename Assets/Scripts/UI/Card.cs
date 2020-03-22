using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {
    [SerializeField] Sprite[] iconList;
    [SerializeField] Sprite[] faceList;
    [SerializeField] Text[] labels;
    [SerializeField] Image[] icons;
    [SerializeField] Image[] faces;
    [SerializeField] Image back;

    [SerializeField] int icon;
    [SerializeField] int value;

    Coroutine flipCoroutine;

    public int Value { get { return value; } }

    private bool flipped;
    public bool Flip {
        get {
            return flipped;
        }
        set {
            if (flipped == value) return;
            flipped = value;
            if (flipCoroutine != null) StopCoroutine(flipCoroutine);
            flipCoroutine = StartCoroutine(DoFlip(value));
        }
    }
    public Vector3 targetPos;

    private void Awake() {
        targetPos = transform.localPosition;
        Set(icon, value);
    }

    private void FixedUpdate() {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, .7f);
    }

    public void Set(int _icon, int _value) {
        icon = _icon;
        value = _value;
        string txt = value.ToString();
        if (value == 1) txt = "A";
        if (value == 11) txt = "J";
        if (value == 12) txt = "Q";
        if (value == 13) txt = "K";
        foreach (Text label in labels) {
            label.text = txt;
        }
        foreach (Image spr in icons) {
            spr.sprite = iconList[icon];
        }
        
        if (value > 10) {
            foreach (Image f in faces) {
                f.sprite = faceList[value - 11];
                f.gameObject.SetActive(true);
            }
        } else {
            foreach (Image f in faces) f.gameObject.SetActive(false);
        }
    }

    private IEnumerator DoFlip(bool f) {
        // Hide
        float perc = 0;
        while (perc < 1) {
            transform.localScale = new Vector3(1 - perc, 1, 1);
            perc += Time.deltaTime * 4;
            yield return null;
        }
        back.gameObject.SetActive(!f);
        // Show
        perc = 0;
        while (perc < 1) {
            transform.localScale = new Vector3(perc, 1, 1);
            perc += Time.deltaTime * 4;
            yield return null;
        }
        transform.localScale = new Vector3(1, 1, 1);
    }
}
