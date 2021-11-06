using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathListener : MonoBehaviour
{
    void Start()
    {
        GlobalEventSubject.playerDeathEvent += OnPlayerDeath;
    }
    
    private void OnDisable() {
        GlobalEventSubject.playerDeathEvent -= OnPlayerDeath;
    }
    void OnPlayerDeath() {
        StartCoroutine(loadTitle());
    }

    IEnumerator loadTitle()
    {
        yield return new WaitForSeconds(3f);
        SceneHandler.AttemptLoadScene(0);
    }
}
