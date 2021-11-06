using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : WeaponHandler
{
    [SerializeField] PlayerBehavior player;
    [SerializeField] Animator animator;
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void OnTriggerEnter(Collider other)
    {
        if (animator != null)
        {
            int momentum = Mathf.RoundToInt(animator.GetFloat(Hashes.momentum));
            int attackInput = Mathf.RoundToInt(animator.GetFloat(Hashes.attackInput));
            float momentumMultiplier = 1 + player.playerSettings.momentumMultiplierAddition * Mathf.Abs(momentum);
            bool strongAttack = false;
            if (momentum != 0)
            {
                if (Mathf.Sign(momentum) == 1 && attackInput == 0
            || Mathf.Sign(momentum) == -1 && attackInput == 1
            || attackInput == 2 || attackInput == 3)
                {
                    strongAttack = true;
                }
            }
            float strongMultiplier = 1 + (strongAttack ? player.playerSettings.strongAttackMultiplierAddition * Mathf.Abs(momentum) : 0);
            getHitData.dmg = Mathf.RoundToInt(weaponSettings.damage * momentumMultiplier * strongMultiplier);
        }
        // print("damage: " + getHitData.dmg);
        // Before we trigger the damage, we have to modify the damage based on the current momentum and attack. 
        // Each level of momentum gives an additive 20% damage boost. 
        // A strong attack grants a multiplicative 100% damage boost. 
        base.OnTriggerEnter(other);
    }
}
