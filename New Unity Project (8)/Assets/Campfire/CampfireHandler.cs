using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireHandler : MonoBehaviour
{
    Transform player;
    HealthHandler playerHealthHandler;
    public int healRate = 3;
    public float healDelay = 1f;
    public int woodConsumeRate = 1;
    [HideInInspector] public float sootLevel;
    public float sootAccumulationRate;
    public float sootCleansingRate;
    [SerializeField] float cullDistance;
    float timer;
    int healthStorage;
    [HideInInspector] public HashSet<Transform> objectsNearCampfire = new HashSet<Transform>();
    OnHealParams onHealParams;

    GameObject currentBar;
    public GameObject sootBarObject;
    public float barHeight;
    MaterialPropertyBlock matBlock;
    MeshRenderer meshRenderer;
    Camera mainCamera;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        timer = healDelay;
        onHealParams = new OnHealParams();
        onHealParams.healAmount = healRate;
        onHealParams.woodConsumed = woodConsumeRate;
        onHealParams.campfire = this;
        StartCoroutine(Heal());
    }

    private void OnEnable()
    {
        if (currentBar == null)
        {
            currentBar = Instantiate(sootBarObject, transform.position + Vector3.up * barHeight, Quaternion.identity, transform);
            matBlock = new MaterialPropertyBlock();
            meshRenderer = currentBar.GetComponent<MeshRenderer>();
            mainCamera = Camera.main;
            sootLevel = 0f;
        }
    }

    IEnumerator Heal()
    {
        while (true)
        {
            yield return new WaitForSeconds(healDelay);
            if (player == null) continue;
            if ((player.transform.position - transform.position).sqrMagnitude > cullDistance * cullDistance)
            {
                currentBar.SetActive(false);
                // cleanse soot
                sootLevel = Mathf.Max(sootLevel - sootCleansingRate, 0);
                continue;
            }

            // Since we're not in cull distance, we need to see the soot bar. 
            currentBar.SetActive(true);
            // If we're within distance for soot to accumulate, check if there's too much soot already. 
            if (sootLevel > 0.99f) continue;

            // if we made it this far we can accumulate soot and heal. 
            // However, soot accumulation should be left to the people healing from the camp. 
            // So that soot only accumulates when they heal. 
            foreach (Transform obj in objectsNearCampfire)
            {
                if (obj != null)
                obj.BroadcastMessage("OnHeal", onHealParams, SendMessageOptions.DontRequireReceiver);
            }
            
            UpdateParams();
        }
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        print(other.name + " has entered the campfire.");
        objectsNearCampfire.Add(other.transform);
    }
    
    private void OnTriggerExit(Collider other) {
        print(other.name + " has exited the campfire.");
        objectsNearCampfire.Remove(other.transform);
    }

    private void UpdateParams()
    {
        meshRenderer.GetPropertyBlock(matBlock);
        matBlock.SetFloat("_Fill", sootLevel / 1);
        meshRenderer.SetPropertyBlock(matBlock);
    }

    private void AlignCamera()
    {
        if (mainCamera != null)
        {
            var camXform = mainCamera.transform;
            var forward = currentBar.transform.position - camXform.position;
            forward.Normalize();
            var up = Vector3.Cross(forward, camXform.right);
            currentBar.transform.rotation = Quaternion.LookRotation(forward, up);
        }
    }
    void Update()
    {
        AlignCamera();
    }
}
