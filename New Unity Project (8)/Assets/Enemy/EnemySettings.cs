using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Settings/EnemySettings", fileName="EnemySettings")]
public class EnemySettings : ScriptableObject
{
    [Min(0f)] public int health;
    public float sprintSpeed;
    public float walkSpeed;
    [Min(0f)] public float alertRadius;
    [Min(0f)] public float attackRadius;
    [Min(0f)] public float attackDelay;
    [Min(0.01f)] public float minWalkRadius;
    [Min(0.01f)] public float maxWalkRadius;
    [Range(0f, 1f)] public float chanceToStand;
    
    [Min(0f)] public float minStandTime;
    [Min(0f)] public float maxStandTime;
    [Min(0f)] public float despawnTime;
    public GameObject ragdoll;
}
