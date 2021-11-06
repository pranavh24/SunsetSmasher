using System;
using UnityEngine;

public class InputEventSubject
{
    
    public static event Action<InputEventArgs> moveEvent;
    public static void RaiseMoveEvent(InputEventArgs args) {
        moveEvent?.Invoke(args);
    }
    
    public static event Action<InputEventArgs> jumpEvent;
    public static void RaiseJumpEvent(InputEventArgs args) {
        jumpEvent?.Invoke(args);
    }
    
    public static event Action<InputEventArgs> attackEvent;
    public static void RaiseAttackEvent(InputEventArgs args) {
        attackEvent?.Invoke(args);
    }
    
    public static event Action<InputEventArgs> throwEvent;
    public static void RaiseThrowEvent(InputEventArgs args) {
        throwEvent?.Invoke(args);
    }
    
    public static event Action<InputEventArgs> landOnGroundEvent;
    public static void RaiseLandOnGroundEvent(InputEventArgs args) {
        landOnGroundEvent?.Invoke(args);
    }
    
    public static event Action<InputEventArgs> dashEvent;
    public static void RaiseDashEvent(InputEventArgs args) {
        dashEvent.Invoke(args);
    }
    
    public class InputEventArgs {
        public float cameraYRotation;
        public Vector2 localMovementDirection;
        public bool isOnGround;
        public Transform playerTransform;
        public Stance currentStance;
        public byte momentum; // Only needs to be a byte because it only has 3 levels. 
        public PlayerBehavior player;
        public Animator playerAnim;
    }
    
    public enum Stance {
        Neutral, Running, Jumping, LeftSwing, RightSwing, LeftRadial, RightRadial, Throw
    }
}
