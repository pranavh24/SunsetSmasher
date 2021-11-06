using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreHandler : MonoBehaviour
{
    int score;
    TextMeshProUGUI scoreTextObject;
    string originalText;
    // Start is called before the first frame update
    void Start()
    {
        scoreTextObject = GetComponent<TextMeshProUGUI>();
        score = 0;
        originalText = scoreTextObject.text;
        scoreTextObject.text = originalText + score;
        EnemyEventSubject.enemyDeathEvent += OnEnemyDeath;
    }

    void OnEnemyDeath(EnemyEventSubject.EnemyEventArgs args, Transform killer)
    {
        if (killer != null && killer.tag == "Player") // if the killer is a player
        {
            score++;
            scoreTextObject.text = originalText + score;
        }

    }
}
