using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateBehavior : StateMachineBehaviour
{
    [SerializeField]
    float inputDelay;
    bool waitingForInput;
    PlayerBehavior player;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
        {
            player = animator.GetComponent<PlayerBehavior>(); // The player animator should be guaranteed to be on a player
        }
        waitingForInput = false;
        animator.SetFloat(Hashes.attackInput, player.playerData.bufferedAttackInput);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= inputDelay && !waitingForInput)
        {
            animator.SetBool(Hashes.attackTriggered, false);
            waitingForInput = true;
            int attackinput = Mathf.RoundToInt(animator.GetFloat(Hashes.attackInput));
            int momentum = Mathf.RoundToInt(animator.GetFloat(Hashes.momentum));
            switch (attackinput)
            {
                case 1: // right
                    if (momentum >= 0)
                    {
                        momentum = Mathf.Min(3, momentum + 1);
                    }
                    else
                    {
                        momentum = 1;
                    }
                    break;
                case 2:
                    momentum = 0;
                    break;
                case 3:
                    momentum = 0;
                    break;
                default: // left
                    if (momentum <= 0)
                    {
                        momentum = Mathf.Max(-3, momentum - 1);
                    }
                    else
                    {
                        momentum = -1;
                    }
                    break;
            }
            player.playerData.bufferedMomentum = momentum;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Deal with the results of the attack we just finished
        // set momentum in the state to the intendedMomentum
        if (!animator.GetBool(Hashes.attackTriggered))
        { // If we haven't buffered an attack or used a strong attack... 
            animator.SetFloat(Hashes.momentum, 0);
            player.playerData.bufferedMomentum = 0;

            // we don't want to set intendedMomentum if the player is performing a strong attack
            // even though strong attacks cancel momentum
            // because the player may have chained another attack, which sets intendedMomentum in PlayerBehavior. 
            // We are only sure that the player hasn't set intendedMomentum if an attack hasn't been triggered. 
        } else {
            animator.SetFloat(Hashes.momentum, player.playerData.bufferedMomentum);
        }
    }

    private bool IsApprox(float original, float num, float error)
    {
        return Mathf.Abs(original - num) < error;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    private void OnAttackTriggered(InputEventSubject.InputEventArgs args)
    {

    }
}
