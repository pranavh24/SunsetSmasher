using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHashes
{
    public static int paramHealth = Animator.StringToHash("Health");
    public static int paramSpeed = Animator.StringToHash("Speed");
    public static int paramGotHit = Animator.StringToHash("GotHit");
    public static int paramAttackTrigerred = Animator.StringToHash("AttackTriggered");
    public static int stateIdleRun = Animator.StringToHash("Idle Run Blend");
    public static int stateAttack = Animator.StringToHash("Attack");
    public static int stateTakeDamage = Animator.StringToHash("Take Damage");
    public static int stateDeath = Animator.StringToHash("Death");
    
}
