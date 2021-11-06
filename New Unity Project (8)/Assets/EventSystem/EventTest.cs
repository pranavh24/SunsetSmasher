using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTest : MonoBehaviour
{
    [SerializeField]
    UnityEvent OnKPressed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {
            OnKPressed.Invoke();
        }
    }
    
    public void PrintSpace() {
        print("Space!");
    }
}