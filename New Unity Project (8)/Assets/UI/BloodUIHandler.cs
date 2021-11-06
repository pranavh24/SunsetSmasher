using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BloodUIHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMesh;
    
    string originalText;
    // Start is called before the first frame update
    void Start()
    {
        originalText = textMesh.text;
        GlobalEventSubject.playerDataUpdatedEvent += OnPlayerUpdated;
    }

    void OnPlayerUpdated(PlayerData playerData) {
        textMesh.text = originalText + playerData.bloodAmount;
    }
}
