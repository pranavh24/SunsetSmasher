using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HealthHandler))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public float despawnDistance = 30f;
    [HideInInspector] public GameObject player;

    [SerializeField] Rigidbody rb;
    [SerializeField] public EnemySettings enemySettings;
    public WeaponHandler weapon;
    public EnemyData enemyData;
    float attackTimer;
    private Animator anim;
    Coroutine currentRoutine;
    NavMeshAgent agent;
    [HideInInspector] public HealthHandler healthHandler;
    // events
    EnemyEventSubject.EnemyEventArgs enemyEventArgs;
    private void OnEnable()
    {
        if (enemyData == null) // This is our first init. 
        {
            player = GameObject.Find("Player");
            anim = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            attackTimer = -1f;
            enemyData = new EnemyData();
            enemyEventArgs = new EnemyEventSubject.EnemyEventArgs();
            enemyEventArgs.enemyBehavior = this;
            enemyEventArgs.enemyTransform = transform;
            enemyData.enemyState = EnemyState.Wandering;
        }
        healthHandler = GetComponent<HealthHandler>();
        healthHandler.maxHealth = enemySettings.health;
        healthHandler.currentHealth = enemySettings.health;
        currentRoutine = StartCoroutine(WanderRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    void Update()
    {
        if (PauseHandler.paused) return;
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        if (enemyData.enemyState == EnemyState.Dead) return;
        if (enemyData.enemyState == EnemyState.Stunned) return;
        if (player == null)
        {
            if (enemyData.enemyState != EnemyState.Wandering)
            {
                ChangeEnemyState(EnemyState.Wandering);
            }
            return;
        }
        float sqrDistToPlayer = (transform.position - player.transform.position).sqrMagnitude;
        if (sqrDistToPlayer > enemySettings.alertRadius * enemySettings.alertRadius)
        {
            if (enemyData.enemyState != EnemyState.Wandering)
            {
                ChangeEnemyState(EnemyState.Wandering);
            }
        }
        else if (sqrDistToPlayer > enemySettings.attackRadius * enemySettings.attackRadius)
        {
            if (enemyData.enemyState != EnemyState.Chasing)
            {
                ChangeEnemyState(EnemyState.Chasing);
            }
        }
        else
        {
            if (enemyData.enemyState != EnemyState.InCombat)
            {
                ChangeEnemyState(EnemyState.InCombat);
            }
        }
        if ((transform.position - player.transform.position).sqrMagnitude > despawnDistance * despawnDistance)
        {
            OnDieData falseData = new OnDieData();
            falseData.deadObject = transform;
            falseData.killer = null;
            OnDie(falseData);
        }
    }

    Coroutine currentKnockbackRoutine;
    void OnGetHit(GetHitData getHitData)
    {
        // ChangeEnemyState(EnemyState.Stunned);
        // Knockback
        currentKnockbackRoutine = StartCoroutine(KnockbackRoutine(getHitData));
    }

    IEnumerator KnockbackRoutine(GetHitData getHitData)
    {
        anim.SetTrigger(EnemyHashes.paramGotHit);
        Vector3 knockBackVelocity = new Vector3(
            transform.position.x - getHitData.sourcePosition.x,
            0,
            transform.position.z - getHitData.sourcePosition.z
        );
        knockBackVelocity = knockBackVelocity.normalized * getHitData.knockbackStrength;
        agent.updatePosition = false;
        agent.updateRotation = false;
        for (float i = 0; i < getHitData.knockbackDuration; i += Time.deltaTime)
        {
            rb.MovePosition(rb.position + knockBackVelocity * Time.deltaTime);
            if (PauseHandler.paused) yield return new WaitWhile(() => PauseHandler.paused);
            yield return new WaitForEndOfFrame();
        }
        agent.nextPosition = rb.position;
        agent.updatePosition = true;
        agent.updateRotation = true;
    }
    // Called by HealthManager when health reaches 0
    void OnDie(OnDieData onDieData)
    {
        gameObject.SetActive(false);
        GameObject ragdollInstance = Instantiate(enemySettings.ragdoll, transform.position, transform.rotation);
        // Apply force
        if (onDieData.killer != null)
        {
            Vector3 forceDirection = -(onDieData.killer.position - transform.position).normalized;
            ragdollInstance.GetComponent<Rigidbody>().velocity = forceDirection * 20;
        }
        EnemyEventSubject.RaiseEnemyDeathEvent(enemyEventArgs, onDieData.killer);

    }

    public void ChangeEnemyState(EnemyState newState)
    {
        enemyData.enemyState = newState;
        StopCoroutine(currentRoutine);
        switch (newState)
        {
            case EnemyState.Wandering:
                anim.SetFloat(EnemyHashes.paramSpeed, enemySettings.walkSpeed);
                currentRoutine = StartCoroutine(WanderRoutine());
                break;
            case EnemyState.Chasing:
                anim.SetFloat(EnemyHashes.paramSpeed, enemySettings.sprintSpeed);
                currentRoutine = StartCoroutine(ChaseRoutine());
                break;
            case EnemyState.InCombat:
                currentRoutine = StartCoroutine(CombatRoutine());
                break;
            case EnemyState.Dead:
                break;

        }
    }
    NavMeshHit navMeshHit;
    IEnumerator WanderRoutine()
    {
        // print("Starting wander routine");
        while (true)
        {

            if (Random.value < enemySettings.chanceToStand)
            { // Stand
                agent.speed = 0;
                anim.SetFloat(EnemyHashes.paramSpeed, 0);
                yield return new WaitForSeconds(Random.Range(enemySettings.minStandTime, enemySettings.maxStandTime));
                continue;
            }
            agent.speed = enemySettings.walkSpeed;
            anim.SetFloat(EnemyHashes.paramSpeed, enemySettings.walkSpeed);

            // Pick a random spot on the navmesh to rove to
            float randomWalkRadius = Random.Range(enemySettings.minWalkRadius, enemySettings.maxWalkRadius);
            Vector2 randomDirection;
            do
            {
                randomDirection = Random.insideUnitCircle.normalized;
            } while (randomDirection == Vector2.zero);
            Vector2 randomPosition = randomWalkRadius * randomDirection;
            Vector3 newPosition = new Vector3(
                transform.position.x + randomPosition.x,

                transform.position.y,
                transform.position.z + randomPosition.y
            );
            NavMesh.SamplePosition(newPosition, out navMeshHit, randomWalkRadius, NavMesh.AllAreas);

            // If the new position is too close, try again. Prevents edge-skirting. May hinder enemies from climbing ramps while wandering. 
            if ((transform.position - navMeshHit.position).sqrMagnitude < enemySettings.minWalkRadius * enemySettings.minWalkRadius) continue;
            // Move to the new positoin
            agent.SetDestination(navMeshHit.position);
            yield return new WaitUntil(() => agent.stoppingDistance > agent.remainingDistance);
        }
        // yield return null;
    }
    IEnumerator ChaseRoutine()
    {
        anim.SetFloat(EnemyHashes.paramSpeed, enemySettings.sprintSpeed);
        agent.speed = enemySettings.sprintSpeed;
        while (true)
        {
            // Set the navmesh destination to the player
            NavMesh.SamplePosition(player.transform.position, out navMeshHit, 4f, NavMesh.AllAreas);
            agent.SetDestination(navMeshHit.position);

            // Wait half a second so we don't overload the processor
            // May want to run this in a separate thread eventually
            // so excessive pathfinding doesn't interrupt gameplay
            yield return new WaitForSeconds(.5f);
        }
        // yield return null;
    }
    IEnumerator CombatRoutine()
    {
        anim.SetFloat(EnemyHashes.paramSpeed, 0);
        agent.speed = 0;
        while (true)
        {
            if (attackTimer <= 0)
            {
                anim.SetTrigger(EnemyHashes.paramAttackTrigerred);
                attackTimer = enemySettings.attackDelay;
            }
            yield return new WaitForEndOfFrame();
        }
        // yield return null;
    }
}
