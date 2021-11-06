using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Void Event Channel")]
public class VoidEventSubject:ScriptableObject
{
    
    // public static VoidEventSubject subject; 
    public UnityAction voidEvent;
    
    public void RaiseVoidEvent() {
        voidEvent?.Invoke();
    }
}
