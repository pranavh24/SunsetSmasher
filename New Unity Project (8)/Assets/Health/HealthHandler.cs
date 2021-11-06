using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    [SerializeField] GameObject healthBarObject;
    [SerializeField] int healthHeight;
    GameObject currentBar;
    Collider collider;
    public int _currentHealth;
    public int currentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = value;
            UpdateParams();
        }
    }
    public int _maxHealth = 100;
    public int maxHealth
    {
        get { return _maxHealth; }
        set
        {
            _maxHealth = value;
            UpdateParams();
        }
    }
    bool invulnerable = false;
    Camera mainCamera;
    MeshRenderer meshRenderer;
    private MaterialPropertyBlock matBlock;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {

    }

    private void OnEnable()
    {
        if (currentBar == null)
        {
            currentBar = Instantiate(healthBarObject, transform.position + Vector3.up * healthHeight, Quaternion.identity, transform);
            matBlock = new MaterialPropertyBlock();
            meshRenderer = currentBar.GetComponent<MeshRenderer>();
            mainCamera = Camera.main;
            collider = GetComponent<Collider>();
            if (collider is null) print("No collider attached. Object cannot be damaged");
            currentHealth = maxHealth; // Automatically updates health bar
        }

    }
    void Update()
    {
        AlignCamera();
    }

    void OnGetHit(GetHitData getHitData)
    {
        if (!invulnerable)
        {
            currentHealth = Mathf.Max(0, currentHealth - getHitData.dmg);
        }
        if (currentHealth == 0)
        {
            OnDieData onDie = new OnDieData();
            onDie.deadObject = transform;
            onDie.killer = getHitData.source;
            BroadcastMessage("OnDie", onDie, SendMessageOptions.DontRequireReceiver);
        }
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

    private void UpdateParams()
    {
        meshRenderer.GetPropertyBlock(matBlock);
        matBlock.SetFloat("_Fill", currentHealth / (float)maxHealth);
        meshRenderer.SetPropertyBlock(matBlock);
    }
}
