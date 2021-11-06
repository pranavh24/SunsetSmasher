using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Settings/Weapon Settings", fileName="WeaponSettings")]
public class WeaponSettings : ScriptableObject
{
    public int damage;
    public float knockbackStrength;
    public float knockbackDuration;
    public float stunDuration;
    public string[] strikeableTags;
}
