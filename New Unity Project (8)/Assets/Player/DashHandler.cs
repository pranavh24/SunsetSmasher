using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashHandler : MonoBehaviour
{
    [SerializeField] ParticleSystem dashTrailSystem;
    // Start is called before the first frame update
    private void Start()
    {
        InputEventSubject.dashEvent += OnDashEvent;
    }
    
    private void OnDisable() {
        InputEventSubject.dashEvent -= OnDashEvent;
    }
    void OnDashEvent(InputEventSubject.InputEventArgs args)
    {
        Animator playerAnim = args.playerAnim;
        StartCoroutine(TrailUntilDashEnd(playerAnim));
    }
    IEnumerator TrailUntilDashEnd(Animator playerAnim)
    {
        var emission = dashTrailSystem.emission;
        emission.enabled = true;
        yield return new WaitUntil(() => !playerAnim.GetBool(Hashes.dashTriggered));
        emission.enabled = false;
        yield return null;
    }
}
