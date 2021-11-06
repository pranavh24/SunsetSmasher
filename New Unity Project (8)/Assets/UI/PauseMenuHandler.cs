using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuHandler : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuObject;
    // Start is called before the first frame update
    private void Start() 
    {
        GlobalEventSubject.pauseEvent += OnPaused;
        print(pauseMenuObject.name);
    }
    
    private void OnDisable() {
        GlobalEventSubject.pauseEvent -= OnPaused;
    }
    
    void OnPaused() {
        pauseMenuObject.SetActive(PauseHandler.paused);
    }
}
