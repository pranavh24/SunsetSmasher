using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimHelper : AnimHelper
{
    [SerializeField]
    private ParticleSystem trailSystem;
    [SerializeField]
    private Animator playerAnim;
    [SerializeField] Gradient[] momentumGradients;

    override public void SetWeaponTrigger(int value)
    {
        base.SetWeaponTrigger(value);
        bool onoff = value == 1 ? true : false;
        ParticleSystem.EmissionModule emission = trailSystem.emission;
        emission.enabled = onoff;
        if (onoff)
        {
            ParticleSystem.TrailModule trails = trailSystem.trails;
            trailSystem.Emit(30);
            int momentum = Mathf.RoundToInt(playerAnim.GetFloat(Hashes.momentum));
            trails.colorOverLifetime = new ParticleSystem.MinMaxGradient(momentumGradients[momentum + 3]);
        }
    }
}
