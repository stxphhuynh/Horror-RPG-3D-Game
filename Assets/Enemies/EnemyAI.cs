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

    [Header("Waypoints (assigned at runtime)")]
    public List<Transform> waypoints = new List<Transform>();

    [Header("Audio Setup")]
    public AudioSource audioSource;
    public AudioClip[] ambientGroans;
    public AudioClip attackScream;

    [Header("Health Settings")]
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
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Only patrol if we already have waypoints assigned
        if (waypoints.Count > 0 && agent != null)
        {
            currentWaypointIndex = 0;
            agent.SetDestination(waypoints[0].position);
        }

        ScheduleNextGroan();
    }

    void Update()
    {
        // knockback logic
        if (isKnockedBack)
        {
            return;
        }

        // 1. RANDOM AMBIENT SOUNDS
        if (Time.time >= nextGroanTime)
        {
            PlayRandomGroan();
            ScheduleNextGroan();
        }

        // 2. MOVEMENT ANIMATION
        bool isMoving = agent != null && agent.velocity.magnitude > 0.1f && !agent.isStopped;
        animator.SetBool("isMoving", isMoving);

        // 3. LOGIC CHECK
        if (player != null && agent != null)
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
                        if (attackScream != null && audioSource != null)
                            audioSource.PlayOneShot(attackScream);
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

    // called by WaveSpawner right after spawning
    public void AssignWaypoints(Transform[] points)
    {
        if (points == null || points.Length == 0 || agent == null)
            return;

        waypoints = new List<Transform>(points);
        currentWaypointIndex = 0;
        agent.SetDestination(waypoints[0].position);
    }

    // --- DAMAGE SYSTEM ---
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
        Destroy(gameObject);
    }

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
        if (player == null) return;

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

        float distance = (forceOverride > 0f) ? forceOverride : knockbackForce;
        float duration = (durationOverride > 0f) ? durationOverride : knockbackDuration;

        Vector3 direction = (transform.position - sourcePosition).normalized;
        direction.y = 0f;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + direction * distance;

        bool useAgent = (agent != null && agent.isOnNavMesh);
        bool prevStopped = useAgent ? agent.isStopped : false;
        if (useAgent) agent.isStopped = true;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);

            Vector3 pos = Vector3.Lerp(startPos, endPos, lerp);
            float height = Mathf.Sin(lerp * Mathf.PI) * 0.5f;
            pos.y += height;

            if (useAgent)
                agent.Warp(pos);
            else
                transform.position = pos;

            yield return null;
        }

        if (useAgent)
            agent.isStopped = prevStopped;

        isKnockedBack = false;
    }
}
