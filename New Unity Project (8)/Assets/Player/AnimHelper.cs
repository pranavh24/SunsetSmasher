using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimHelper : MonoBehaviour
{
    public Collider weaponTrigger;
    virtual public void SetWeaponTrigger(int value)
    {
        bool onoff = value == 1 ? true : false;
        weaponTrigger.enabled = onoff;
    }
}
