using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseHandler : MonoBehaviour
{
    public static bool paused;
    // Start is called before the first frame update
    void Start()
    {
        SetPause(false);
    }
    void OnPause()
    {
        SetPause(!paused);
    }
    
    public static void SetPause(bool willPause) {
        Debug.Log("Pause set to " + willPause);
        paused = willPause;
        if (paused)
        {
            Time.timeScale = 0f;
            GlobalEventSubject.RaisePauseEvent();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            GlobalEventSubject.RaisePauseEvent();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
