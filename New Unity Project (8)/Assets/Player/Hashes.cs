using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hashes
{
    public static int hasWeapon = Animator.StringToHash("HasWeapon"); 
    public static int isJumping = Animator.StringToHash("IsJumping"); 
    public static int onChannel = Animator.StringToHash("OnChannel"); 
    public static int momentum = Animator.StringToHash("Momentum"); 
    public static int attackInput = Animator.StringToHash("AttackInput"); 
    public static int height = Animator.StringToHash("Height"); 
    public static int isRunning = Animator.StringToHash("IsRunning"); 
    public static int attackTriggered = Animator.StringToHash("IsAttacking"); 
    public static int speed = Animator.StringToHash("Speed"); 
    public static int dashTriggered = Animator.StringToHash("IsDashing");
    public static int dashTimer = Animator.StringToHash("DashTimer");
    public static int stateAttackBlendTree = Animator.StringToHash("Attack Blend Tree");
    public static int stateJump = Animator.StringToHash("Jump Blend Tree");
    public static int stateRun = Animator.StringToHash("Idle Run Blend");
    public static int stateDash = Animator.StringToHash("Dashing");
    public static int maskPlayer = LayerMask.NameToLayer("Player");
    
}
