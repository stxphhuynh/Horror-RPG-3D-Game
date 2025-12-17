using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Setup")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;
    public List<Transform> waypoints;

    [Header("Audio Setup")]
    public AudioSource audioSource;
    public AudioClip[] ambientGroans;
    public AudioClip attackScream;

    [Header("Health Settings")] // NEW
    public int maxHealth = 100;
    private int currentHealth;

    [Header("AI Settings")]
    public float detectionRange = 10f;
    public float attackRange = 2.0f;
    public float chaseSpeed = 5f;
    public float patrolSpeed = 3f;

    // Timers
    private float nextGroanTime;
    private float lastAttackSoundTime;
    public float attackSoundCooldown = 1.5f;

    private int currentWaypointIndex = 0;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackDuration = .2f;

    private bool isKnockedBack = false;

    void Start()
    {
        currentHealth = maxHealth; // Initialize Health

        // Fail-safes
        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (waypoints.Count > 0) agent.SetDestination(waypoints[0].position);
        ScheduleNextGroan();
    }

    void Update()
    {
     

        // 1. RANDOM AMBIENT SOUNDS
        if (Time.time >= nextGroanTime)
        {
            PlayRandomGroan();
            ScheduleNextGroan();
        }

        // 2. MOVEMENT ANIMATION
        bool isMoving = agent.velocity.magnitude > 0.1f && !agent.isStopped;
        animator.SetBool("isMoving", isMoving);

        // 3. LOGIC CHECK
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < detectionRange)
            {
                animator.SetBool("isChasing", true);

                if (distanceToPlayer <= attackRange)
                {
                    // >>> ATTACK <<<
                    agent.isStopped = true;
                    animator.SetBool("isAttacking", true);
                    LookAtPlayer();

                    if (Time.time > lastAttackSoundTime + attackSoundCooldown)
                    {
                        if (attackScream != null) audioSource.PlayOneShot(attackScream);
                        lastAttackSoundTime = Time.time;
                    }
                }
                else
                {
                    // >>> CHASE <<<
                    agent.isStopped = false;
                    animator.SetBool("isAttacking", false);
                    agent.speed = chaseSpeed;
                    agent.SetDestination(player.position);
                }
            }
            else
            {
                // >>> PATROL <<<
                animator.SetBool("isChasing", false);
                animator.SetBool("isAttacking", false);
                agent.isStopped = false;
                agent.speed = patrolSpeed;

                if (agent.remainingDistance < 0.5f && !agent.pathPending && waypoints.Count > 0)
                {
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
                    agent.SetDestination(waypoints[currentWaypointIndex].position);
                }
            }
        }
    }

    // --- NEW: SIMPLE DAMAGE SYSTEM ---

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Enemy Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Instantly remove the enemy from the game
        Destroy(gameObject);
    }

    // TEST TOOL: Click the enemy to deal damage
    void OnMouseDown()
    {
        TakeDamage(20);
    }

    // --- HELPER FUNCTIONS ---
    void PlayRandomGroan()
    {
        if (ambientGroans.Length > 0 && audioSource != null)
        {
            AudioClip clip = ambientGroans[Random.Range(0, ambientGroans.Length)];
            audioSource.PlayOneShot(clip);
        }
    }

    void ScheduleNextGroan()
    {
        nextGroanTime = Time.time + Random.Range(5f, 15f);
    }

    void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void ApplyKnockBack(Vector3 sourcePosition, float forceOverride = -1f, float durationOverride = -1f)
    {
        if (isKnockedBack) return;
        StartCoroutine(KnockbackRoutine(sourcePosition, forceOverride, durationOverride));

    }


    private IEnumerator KnockbackRoutine(Vector3 sourcePosition, float forceOverride, float durationOverride)
    {
        isKnockedBack = true;

        float distance = (forceOverride > 0f) ? forceOverride : knockbackForce;   // treat force as distance now
        float duration = (durationOverride > 0f) ? durationOverride : knockbackDuration;

        // Direction away from the hit
        Vector3 direction = (transform.position - sourcePosition).normalized;
        direction.y = 0f;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + direction * distance;

        // Stop agent movement during knockback
        bool useAgent = (agent != null && agent.isOnNavMesh);
        bool prevStopped = useAgent ? agent.isStopped : false;
        if (useAgent) agent.isStopped = true;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);

            // backward motion
            Vector3 pos = Vector3.Lerp(startPos, endPos, lerp);

            // small arc up & down so it’s visible
            float height = Mathf.Sin(lerp * Mathf.PI) * 0.5f;   // 0.5 = jump height
            pos.y += height;

            if (useAgent)
                agent.Warp(pos);        // instantly place agent on NavMesh
            else
                transform.position = pos;

            yield return null;
        }

        if (useAgent)
            agent.isStopped = prevStopped;

        isKnockedBack = false;
    }
}
