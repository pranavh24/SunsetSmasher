using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] CharacterController controller;
    [SerializeField] public PlayerSettings playerSettings;
    public PlayerData playerData;
    InputEventSubject.InputEventArgs inputEventArgs;
    // Start is called before the first frame update
    void Start()
    {
        playerData = new PlayerData();
        playerData.originalPosition = transform.localPosition;
        playerData.currentHunger = 1f;
        playerData.bloodAmount = 0;
        InputEventSubject.landOnGroundEvent += OnLand;
        playerData.gravity = Physics.gravity * playerSettings.gravStrength;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // We are performing 5 ground checks
        playerData.groundCheckData = new RaycastHit[5];

        // input event arguments
        inputEventArgs = new InputEventSubject.InputEventArgs();
        inputEventArgs.player = this;
        inputEventArgs.playerAnim = GetComponent<Animator>();
        
        EnemyEventSubject.enemyDeathEvent += OnEnemyDeath;
        StartCoroutine(UpdateFunc());
    }
    
    private void OnDisable() {
        InputEventSubject.landOnGroundEvent -= OnLand;
        EnemyEventSubject.enemyDeathEvent -= OnEnemyDeath;
        StopAllCoroutines();
    }

    Vector3 delayedVelocity;
    // Update is called once per frame
    IEnumerator UpdateFunc()
    {
        while (true)
        {
            float dashTimer = animator.GetFloat(Hashes.dashTimer);
            if (dashTimer > 0)
            {
                animator.SetFloat(Hashes.dashTimer, Mathf.Clamp(dashTimer - Time.deltaTime, 0, Mathf.Infinity));
            }
            transform.localPosition = playerData.originalPosition;
            // GroundCheck updates groundCheckData
            GroundCheck();

            // Check if current speed matches player intent
            // print(moveVelocity);
            Vector2 horizontalMoveVelocity = new Vector2(playerData.moveVelocity.x, playerData.moveVelocity.z);
            animator.SetFloat(Hashes.speed, horizontalMoveVelocity.magnitude / playerSettings.maxMoveSpeed);
            animator.SetFloat(Hashes.height, Mathf.Max(playerData.shortestGroundCheckData.distance - controller.height / 2 - playerSettings.groundCheckForgiveness, 0));
            if (animator.GetBool(Hashes.dashTriggered)) // Dash takes priority, cancels gravity and can be performed mid-air
            {
                float dashSpeed = playerSettings.dashDistance / playerSettings.dashDuration;
                playerData.moveVelocity = new Vector3(playerData.inputVec.x, 0, playerData.inputVec.y) / playerSettings.maxMoveSpeed * dashSpeed;
            }
            else if (!animator.GetBool(Hashes.isJumping))
            {
                if (!controller.isGrounded) // If not on ground, employ gravity
                {
                    playerData.moveVelocity += playerData.gravity * Time.deltaTime;
                }
                else if (
                    (
                        animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Hashes.stateAttackBlendTree
                        || animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Hashes.stateDash
                    )
                )
                {
                    playerData.moveVelocity = Vector3.zero;
                    // If you're on the ground, you're not about to jump, and you're either attacking or post-dash, you can't move. 
                }
                else
                {
                    // If you're not about to jump, normal movement
                    // If you were to jump, we would have to not run this code
                    // Or else the jump would be cancelled. 
                    playerData.moveVelocity = new Vector3(
                        Mathf.Lerp(
                            playerData.moveVelocity.x,
                            playerData.inputVec.x,
                            playerSettings.speedControl),
                        -1f, // This setting of the y component cancels jumps. 
                        Mathf.Lerp(
                            playerData.moveVelocity.z,
                            playerData.inputVec.y,
                            playerSettings.speedControl)
                    );
                }
            }
            // Move character
            controller.Move(controller.transform.TransformDirection(playerData.moveVelocity * Time.deltaTime));
            
            // Update stats
            playerData.currentHunger = Mathf.Max(playerData.currentHunger - playerSettings.hungerLoss * Time.deltaTime, 0);
            if (playerData.currentHunger < .001f) {
                OnDieData onDieData = new OnDieData();
                onDieData.killer = null;
                onDieData.deadObject = transform;
                OnDie(onDieData);
            }
            GlobalEventSubject.RaisePlayerDataUpdatedEvent(playerData);
            if (PauseHandler.paused) {
                yield return new WaitWhile(() => PauseHandler.paused);
            }
            yield return new WaitForEndOfFrame();
        }
    }
    // This ground check uses a raycast. This is more reliable for controlling jump times and animations. 
    bool IsOnGround()
    {
        // return controller.isGrounded;
        return playerData.shortestGroundCheckData.distance <= controller.height / 2 + playerSettings.groundCheckForgiveness;
    }

    void GroundCheck()
    {
        bool wasOnGround = IsOnGround();
        // The 5 locations
        Vector3[] startPositions = {
            Vector3.zero,
            Vector3.left * controller.radius,
            Vector3.right * controller.radius,
            Vector3.forward * controller.radius,
            Vector3.down * controller.radius
        };
        Physics.Raycast(
                controller.transform.position + startPositions[0],
                Vector3.down,
                out playerData.groundCheckData[0],
                Mathf.Infinity,
                ~Hashes.maskPlayer
        );
        playerData.shortestGroundCheckData = playerData.groundCheckData[0];
        for (int i = 0; i < playerData.groundCheckData.Length; i++) {
            Physics.Raycast(
                controller.transform.position + startPositions[i],
                Vector3.down,
                out playerData.groundCheckData[i],
                Mathf.Infinity,
                ~Hashes.maskPlayer
            );
            if (playerData.shortestGroundCheckData.distance > playerData.groundCheckData[i].distance)
            {
                playerData.shortestGroundCheckData = playerData.groundCheckData[i];
            }
        }

        if (wasOnGround != IsOnGround() && !animator.GetBool(Hashes.isJumping))
        {
            // TODO: Implement InputEventArgs
            InputEventSubject.RaiseLandOnGroundEvent(new InputEventSubject.InputEventArgs());
        }
        // print(IsOnGround() + ", " + (groundCheckData.distance - controller.height/2));
    }

    void OnMove(InputValue input)
    {
        playerData.inputVec = input.Get<Vector2>() * playerSettings.maxMoveSpeed;
    }

    void OnLand(InputEventSubject.InputEventArgs args)
    {
        // moveVelocity = Vector3.zero;
    }

    void OnJump()
    {
        if (!PauseHandler.paused && IsOnGround() && !animator.GetBool(Hashes.isJumping) && animator.GetCurrentAnimatorStateInfo(0).shortNameHash != Hashes.stateAttackBlendTree)
        {
            animator.SetBool(Hashes.isJumping, true);
            StartCoroutine(GameHelpers.timedFunction(0.5f, () =>
            {
                animator.SetBool(Hashes.isJumping, false);
            }));
            playerData.moveVelocity += Vector3.up * playerSettings.jumpStrength;
            // print(moveVelocity);
        }
    }


    int DetermineAttack(Vector2 inputVec)
    {
        // inputVec should be a unit vector. Since playerData.inputVec is multiplied by speed (which is stupid), 
        // divide playerData.inputVec by its max speed before using it as a parameter. 
        int determinedAttackState;
        // int momentum = Mathf.RoundToInt(animator.GetFloat(Hashes.momentum));
        if (inputVec.y < -playerSettings.inputSensitivity && Mathf.Abs(playerData.bufferedMomentum) > 0.1f) // If input is down
        {
            determinedAttackState = 2; // perform smash attack
        }
        else if (inputVec.y > playerSettings.inputSensitivity && Mathf.Abs(playerData.bufferedMomentum) > 0.1f) // if input is up
        {
            determinedAttackState = 3; // perform uppercut
        }
        else if (inputVec.x > playerSettings.inputSensitivity) // If input is right
        {
            // momentum direction determines whether momemtum builds or cancels
            determinedAttackState = 1; // perform right attack
        }
        else // The only option left within this scope is if the input is left (default)
        {
            determinedAttackState = 0; // perform left attack
        }

        return determinedAttackState;
    }

    void OnFire()
    {
        if (PauseHandler.paused) {
            return;
        }
        if (!animator.GetBool(Hashes.attackTriggered)) // If we're not in the middle of an attack... 
        {
            playerData.bufferedAttackInput = DetermineAttack(
                playerData.inputVec / playerSettings.maxMoveSpeed // input vector
            );
            // Since OnFire is only called OnAttack, we can't reset momentum in this function when the player doesn't attack. 
            // AttackStateBehavior handles the case where the player doesn't chain an attack, setting the momentum to 0. 

            // prime animator to play/replay attack animation
            animator.SetBool(Hashes.attackTriggered, true);

            // AttackStateBehavior will determine what move to use based on the input buffers in PlayerData. 
            // AttackTrigerred will be set to false by the AttackStateBehavior automatically, much like a real trigger. 
        }
    }
    void OnDash()
    {
        if(PauseHandler.paused) return;
        if (animator.GetFloat(Hashes.dashTimer) <= 0 && !animator.GetBool(Hashes.dashTriggered))
        {
            animator.SetBool(Hashes.dashTriggered, true);


            StartCoroutine(GameHelpers.timedFunction(playerSettings.dashDuration, () =>
            {
                animator.SetBool(Hashes.dashTriggered, false);
            }));


            if (IsOnGround())
            {
                animator.SetFloat(Hashes.dashTimer, playerSettings.dashCooldown);
            }
            else
            {
                animator.SetFloat(Hashes.dashTimer, playerSettings.longDashCooldown);
            }


            // The dash is performed so raise the event for everyone. 
            InputEventSubject.RaiseDashEvent(inputEventArgs);
        }
    }

    void OnDie(OnDieData onDieData)
    {
        GlobalEventSubject.RaisePlayerDeathEvent();
        GlobalEventSubject.RaiseStatusMsgEvent("You Died");
        Instantiate(playerSettings.ragdoll, transform.position, transform.rotation);
        Destroy(transform.parent.gameObject);
    }
    
    void OnEnemyDeath(EnemyEventSubject.EnemyEventArgs args, Transform killer) {
        if (killer != null && killer == transform.parent) {
            // draw blood
            playerData.bloodAmount++;
            // board automatically updated in Update. 
        }
    }
}
