using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] protected Transform mainBody;
    [SerializeField] public WeaponSettings weaponSettings;
    protected GetHitData getHitData;
    // Start is called before the first frame update
    virtual protected void Start()
    {
        getHitData = new GetHitData();
        getHitData.dmg = weaponSettings.damage;
        getHitData.knockbackStrength = weaponSettings.knockbackStrength;
        getHitData.knockbackDuration = weaponSettings.knockbackDuration;
        getHitData.stunDuration = 0f;
        getHitData.source = mainBody;
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        getHitData.sourcePosition = transform.position;
        foreach(string tag in weaponSettings.strikeableTags)
        {
            if (other.tag == tag)
            {
                other.BroadcastMessage("OnGetHit", getHitData, SendMessageOptions.DontRequireReceiver);
                return;
            }
        }
    }
}