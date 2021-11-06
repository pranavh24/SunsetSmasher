using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VoidEventListener : MonoBehaviour
{
    public VoidEventSubject voidEventListener = default;
    public UnityEvent OnEventRaised;
    // Start is called before the first frame update
    void Start()
    {
        voidEventListener.voidEvent += Respond;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void Respond() {
        OnEventRaised.Invoke();
    }
}
