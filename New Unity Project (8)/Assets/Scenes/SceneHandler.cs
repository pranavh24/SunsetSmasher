using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneHandler : MonoBehaviour
{
    public static int sceneToLoadIndex;
    void Start() {
        StartCoroutine(LoadSceneRoutine(sceneToLoadIndex));
    }
    
    IEnumerator LoadSceneRoutine(int sceneID) {
        AsyncOperation asyncSceneLoad = SceneManager.LoadSceneAsync(sceneID);
        while(!asyncSceneLoad.isDone) {
            // Do anything you want while we wait for the scene to load
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    
    public static void AttemptLoadScene(string sceneRequested) {
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(sceneRequested);
        if (sceneIndex < 0) {
            Debug.Log("Scene invalid."); 
            return; 
        }
        sceneToLoadIndex = sceneIndex;
        PauseHandler.SetPause(false);

        SceneManager.LoadScene("Loading");
    }
    
    // This function is basically a duplicate of the one above
    // It would be nice if I could efficiently combine the functions
    // Into one. 
    public static void AttemptLoadScene(int sceneRequested) {
        if (sceneRequested >= SceneManager.sceneCountInBuildSettings) {
            Debug.Log("Scene invalid."); 
            return; 
        }
        sceneToLoadIndex = sceneRequested;
        PauseHandler.SetPause(false);
        SceneManager.LoadScene("Loading");
    }
}
