using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour {
    public static bool MessageOpen;
    public static MessageBox Instance { get; private set; }

    [SerializeField] Text messageLabel;
    [SerializeField] GameObject box;
    [SerializeField] Vector3 boxOffset;
    public CanvasGroup GameOver;

    private Vector3 originalBoxPosition;

    void Start() {
        Instance = this;
        originalBoxPosition = box.transform.position;
        box.SetActive(false);
    }

    private void Update() {
        
    }

    public void ShowText(GameObject target, string text, float wait) {
        messageLabel.text = "";
        if (target != null) {
            Vector3 pp = Camera.main.WorldToScreenPoint(target.transform.position);
            box.transform.position = pp + boxOffset;
        }
        else box.transform.position = originalBoxPosition;
        MessageOpen = true;
        StartCoroutine(ProcessMessage(text, wait));
    }

    private IEnumerator ProcessMessage(string text, float wait) {
        box.SetActive(true);
        int chars = 0;
        while (chars < text.Length) {
            chars++;
            messageLabel.text = text.Substring(0, chars);
            yield return null;
        }
        yield return new WaitForSeconds(wait);
        box.SetActive(false);
        MessageOpen = false;
    }
}