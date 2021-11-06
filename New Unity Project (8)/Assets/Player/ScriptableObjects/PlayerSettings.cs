using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Settings/PlayerSettings", fileName="PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    public float maxMoveSpeed;
    public float jumpStrength;
    public float gravStrength;
    public float groundCheckForgiveness;
    public float speedControl;
    public float inputSensitivity;
    public float cameraSensitivity;
    public float turnSpeed;
    public float dashDistance;
    public float dashDuration;
    public float dashCooldown;
    public float longDashCooldown;
    public bool invertY;
    public float hungerRegen;
    public float hungerLoss;
    public float momentumMultiplierAddition = 0.1f;
    public float strongAttackMultiplierAddition = 0.4f;
    public GameObject ragdoll;
}
