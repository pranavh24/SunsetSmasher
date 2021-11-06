using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstantiateCommon : MonoBehaviour
{
    private void Start()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("PlayerUI", LoadSceneMode.Additive);
        StartCoroutine(WaitForUILoad(async));
    }
    
    IEnumerator WaitForUILoad(AsyncOperation async) {
        while(!async.isDone) {
            yield return null;
        }
        GlobalEventSubject.RaisePlayerDataUpdatedEvent(GetComponent<PlayerBehavior>().playerData);
    }
    void DelayedUIUpdate()
    {

    }
}
