using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StatusMsgHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textObj;
    [SerializeField] float fadeDelay = .75f;
    [SerializeField] float fadeDuration = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        GlobalEventSubject.statusMsgEvent += OnStatusMsgEvent;
    }
    
    private void OnDisable() {
        GlobalEventSubject.statusMsgEvent -= OnStatusMsgEvent;
    }
    Coroutine fadeRoutine;
    void OnStatusMsgEvent(string statusMsg) {
        textObj.color = Color.white;
        textObj.text = statusMsg;
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeStatusMsg());
    }
    
    IEnumerator FadeStatusMsg() {
        Color originalCol = textObj.color;
        Color transparentCol = new Color(1, 1, 1, 0);
        yield return new WaitForSeconds(fadeDelay);
        for (float t = 0; t < fadeDuration; t += Time.deltaTime) {
            textObj.color = Color.Lerp(originalCol, transparentCol, t);
            yield return new WaitForEndOfFrame();
        }
    }
}
