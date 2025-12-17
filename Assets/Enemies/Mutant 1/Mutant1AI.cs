using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Mutant1AI : MonoBehaviour
{
    [Header("Setup")]
    public NavMeshAgent agent;
    public Animator animator; // <--- CHANGED to Animator
    public Transform player;
    public List<Transform> waypoints;

    [Header("Settings")]
    public float detectionRange = 10f;
    public float chaseSpeed = 5f;
    public float patrolSpeed = 3f;

    private int currentWaypointIndex = 0;

    void Start()
    {
        // Fail-safe: Get the Animator if you forgot to drag it
        if (animator == null) animator = GetComponent<Animator>();

        // Auto-find player
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (waypoints.Count > 0) agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        // 1. CALCULATE MOVEMENT
        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        // 2. LOGIC & CHASING ANIMATION
        // Check distance to player
        if (player != null && Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            // --- STATE: CHASING ---
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);

            // Tell Animator to RUN
            animator.SetBool("isChasing", true);
        }
        else
        {
            // --- STATE: PATROLLING ---
            agent.speed = patrolSpeed;

            // Tell Animator NOT to run (Walk instead)
            animator.SetBool("isChasing", false);

            // Patrol Logic
            if (agent.remainingDistance < 0.5f && !agent.pathPending && waypoints.Count > 0)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
        }
    }
}