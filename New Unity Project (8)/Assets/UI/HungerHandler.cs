using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HungerHandler : MonoBehaviour
{
    [SerializeField] Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        GlobalEventSubject.playerDataUpdatedEvent += OnPlayerDataUpdated;
    }
    
    void OnPlayerDataUpdated(PlayerData playerData) {
        slider.value = playerData.currentHunger;
    }
    
}
