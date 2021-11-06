using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimeHandler : MonoBehaviour
{
    TextMeshProUGUI textObject;
    string originalText;
    TimeSpan time;
    // Start is called before the first frame update
    private void Start() {
        textObject = GetComponent<TextMeshProUGUI>();
        originalText = textObject.text;
        time = TimeSpan.FromSeconds(0);
        textObject.text = originalText + time.ToString(@"hh\:mm\:ss");
        StartCoroutine(TimeGame());
    }
    // Update is called once per frame
    IEnumerator TimeGame() {
        int timer = 0;
        while (true) {
            yield return new WaitForSeconds(1f);
            timer++;
            time = TimeSpan.FromSeconds(timer);
            textObject.text = originalText + time.ToString(@"hh\:mm\:ss");
        }
    }
}
