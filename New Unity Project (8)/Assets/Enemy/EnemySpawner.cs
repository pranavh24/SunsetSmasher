using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [HideInInspector] public int time;
    public int currentEnemyLevel;
    public GameObject enemy;
    [SerializeField] int numEnemies;
    [SerializeField] WeaponSettings[] weaponLevels;
    [SerializeField] EnemySettings[] enemyLevels;
    [SerializeField] int[] timeThresholds;
    int currentEnemies;
    public static Stack<GameObject> respawnableEnemies;
    public float maxSpawnRadius;
    public float minSpawnRadius;
    public float respawnTryDelay = 2f;
    public int maxRespawnAttempts = 0;
    private Camera mainCam;
    // This enemy spawner implements object pooling for garbage collection efficiency. 
    // 
    private void Start()
    {
        mainCam = Camera.main;
        currentEnemies = 0;
        respawnableEnemies = new Stack<GameObject>();
        EnemyEventSubject.enemyDeathEvent += OnEnemyDie;
        StartCoroutine(AttemptToSpawnEnemy(maxRespawnAttempts));
        time = 0;
        currentEnemyLevel = 0;
        StartCoroutine(KeepTime());
    }
    
    IEnumerator KeepTime() {
        GlobalEventSubject.RaiseStatusMsgEvent("Enemies are now level " + (currentEnemyLevel + 1));
        while(true) {
            yield return new WaitForSeconds(1f);
            time++;
            GlobalEventSubject.RaiseEnemyTimeUpdateEvent((float)time / timeThresholds[timeThresholds.Length - 1]);
            if (time > timeThresholds[currentEnemyLevel]) {
                currentEnemyLevel++;
                GlobalEventSubject.RaiseStatusMsgEvent("Enemies are now level " + (currentEnemyLevel + 1));
            }
            if (currentEnemyLevel == timeThresholds.Length - 1) break;
            // There are no more levels to move up. 
        }
    }
    
    private void OnDisable() {
        EnemyEventSubject.enemyDeathEvent -= OnEnemyDie;
    }

    void OnEnemyDie(EnemyEventSubject.EnemyEventArgs args, Transform killer)
    {
        args.enemyTransform.gameObject.SetActive(false); // Should be set to false before OnEnemyDie was called, but just in case. 
        respawnableEnemies.Push(args.enemyTransform.gameObject);
        currentEnemies--;
        // For now, respawn enemy at the moment of death
    }


    IEnumerator AttemptToSpawnEnemy(int maxAttempts)
    {
        int attemptsCount = 0;
        while (true)
        {
            if (currentEnemies >= numEnemies) { yield return new WaitUntil(() => currentEnemies < numEnemies); }
            else { yield return new WaitForSeconds(respawnTryDelay); }
            Vector3 newPosition = PickPosition() + Vector3.up * 1f;
            // print(newPosition);
            bool inViewFrustrum = false;
            if (mainCam != null)
            {
                Vector3 viewPortPoint = mainCam.WorldToViewportPoint(newPosition);
                inViewFrustrum = (uint)viewPortPoint.x < 1f && (uint)viewPortPoint.y < 1f && viewPortPoint.z > 0f;
            }
            if (newPosition != Vector3.zero && !inViewFrustrum)
            {
                // we can spawn the enemy
                // determine settings based on currentEnemyLevel 
                WeaponSettings weaponSettings = weaponLevels[currentEnemyLevel];
                EnemySettings enemySettings = enemyLevels[currentEnemyLevel];
                
                // Actually spawn enemy
                GameObject spawnedEnemy;
                if (respawnableEnemies.Count > 0) spawnedEnemy = respawnableEnemies.Pop();
                else spawnedEnemy = Instantiate(enemy, newPosition, Quaternion.identity);
                
                // We have to guarantee that the enemy contains an Enemy behavior. 
                // Settings enemySettings before the next update will still allow 
                // enemies to update their health and damage. 
                Enemy enemyScript = spawnedEnemy.GetComponent<Enemy>();
                enemyScript.enemySettings = enemySettings;
                enemyScript.weapon.weaponSettings = weaponSettings;
                
                spawnedEnemy.transform.position = newPosition;
                spawnedEnemy.transform.rotation = Quaternion.Euler(0, Random.value * 360, 0);
                spawnedEnemy.SetActive(true);
                currentEnemies++;
            }
            // enemy health should be set in OnEnable in the enemy script
            if (maxAttempts > 0)
            {
                attemptsCount++;
                if (attemptsCount >= maxAttempts) break;
            }
        }
    }
    NavMeshHit navMeshHit;
    public int pickPositionTries = 5;
    Vector3 PickPosition()
    {
        bool foundNewPos = false;
        for (int i = 0; i < pickPositionTries; i++)
        {
            Vector2 newRelativePosFlat = Random.insideUnitCircle.normalized;
            Vector3 newRelativePos = new Vector3(newRelativePosFlat.x, 0, newRelativePosFlat.y);
            newRelativePos *= Random.Range(minSpawnRadius, maxSpawnRadius);
            foundNewPos = NavMesh.SamplePosition(transform.position + newRelativePos, out navMeshHit, maxSpawnRadius, NavMesh.AllAreas);
            if (foundNewPos) return navMeshHit.position;
        }

        return Vector3.zero;
    }
}
