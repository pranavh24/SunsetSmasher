using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int bufferedAttackInput;
    private int _bufferedMomentum;
    public int bufferedMomentum {
        get {
            return _bufferedMomentum;
        }
        
        set {
            _bufferedMomentum = value;
        }
    }
    public Vector3 gravity; 
    public Vector3 moveVelocity; 
    public Vector2 inputVec; 
    public Vector3 originalPosition; 
    public RaycastHit[] groundCheckData; 
    public RaycastHit shortestGroundCheckData;
    public int bloodAmount;
    public int currentHealth;
    public float currentHunger;
    public PlayerData() {
        bufferedAttackInput = 0;
        bufferedMomentum = 0;
        moveVelocity = Vector3.zero;
        inputVec = Vector2.zero;
        originalPosition = Vector3.zero;
        currentHunger = 1f;
        bloodAmount = 0;
    }
}
