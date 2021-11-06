using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleHandler : MonoBehaviour
{
    private void Start() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    // Start is called before the first frame update
    public void AttemptLoad(string levelToLoad) {
        
        SceneHandler.AttemptLoadScene(levelToLoad);
    }
    
    public void OnClickExit() {
        Application.Quit();
    }
}
