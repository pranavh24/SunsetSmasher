using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyScrollHandler : MonoBehaviour
{
    [SerializeField] RectTransform rawImageRect;
    [SerializeField] RectTransform maskRect;
    float distToMove;
    float width;
    // Start is called before the first frame update
    void Start()
    {
        width = rawImageRect.sizeDelta.x;
        GlobalEventSubject.enemyTimeUpdateEvent += OnEnemyTimeUpdate;
    }
    
    private void OnDisable() {
        GlobalEventSubject.enemyTimeUpdateEvent -= OnEnemyTimeUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnEnemyTimeUpdate(float percentDone) {
        // print("updating scroll position: " + percentDone);
        rawImageRect.anchoredPosition = new Vector2(-percentDone * width, 0);
    }
}
