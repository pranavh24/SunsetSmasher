using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerBehavior))]
public class PlayerCamHandler : MonoBehaviour
{
    [SerializeField]
    Transform followTarget;
    [SerializeField]
    Animator animator;
    PlayerBehavior playerBehavior;
    void Start()
    {
        playerBehavior = GetComponent<PlayerBehavior>();
        trueRot = Mathf.Clamp(transform.localEulerAngles.x, -60f, 60f);
    }

    void Update()
    {
        if (PauseHandler.paused) return;
        int currentState = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        if ((currentState == Hashes.stateRun) && animator.GetFloat(Hashes.speed) > 0.01f)
        {
            Quaternion storedGlobalRotation = followTarget.rotation;
            // This sets follow target out of rotation
            transform.parent.rotation = Quaternion.Lerp(
                transform.parent.rotation, 
                Quaternion.Euler(0, followTarget.eulerAngles.y, 0), 
                playerBehavior.playerSettings.turnSpeed); 
            followTarget.rotation = storedGlobalRotation; // Sets follow target/camera rotation to original rot
        }
    }
    
    float trueRot;
    // Start is called before the first frame update
    void OnLook(InputValue input)
    {
        if (PauseHandler.paused) return;
        if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != Hashes.stateAttackBlendTree)
        {
            Vector2 inputVec = input.Get<Vector2>();
            followTarget.eulerAngles += inputVec.x * Vector3.up * playerBehavior.playerSettings.cameraSensitivity * .25f;
            Vector3 locEulerAngles = followTarget.localEulerAngles;
            
            float deltaRotY =  inputVec.y * playerBehavior.playerSettings.cameraSensitivity * .25f;
            deltaRotY *= playerBehavior.playerSettings.invertY ? 1 : -1;
            trueRot = Mathf.Clamp(trueRot + deltaRotY, -20f, 60f);
            float newRot = trueRot < 0 ? 360 + trueRot : trueRot;
            // print(newRot);
            followTarget.localRotation = Quaternion.Euler(
                newRot, 
                locEulerAngles.y, 
                locEulerAngles.z
            );
            // print("Tilted by " + Mathf.Clamp(
            //     inputVec.y * playerBehavior.playerSettings.cameraSensitivity * .25f, 
            //     -60f - locEulerAngles.x, 60f - locEulerAngles.x
            // ));
        }

    }

    void OnFire()
    {
        if (PauseHandler.paused) return;
        int currentState = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        if (currentState != Hashes.stateAttackBlendTree && currentState != Hashes.stateJump)
        {
            Quaternion storedGlobalRotation = followTarget.rotation;
            transform.parent.rotation = Quaternion.Euler(0, followTarget.eulerAngles.y, 0);
            followTarget.rotation = storedGlobalRotation;
        }
    }

    void OnMove()
    {
        
    }
    
    void OnDie(OnDieData onDieData) {
        followTarget.parent = null;
    }
}
