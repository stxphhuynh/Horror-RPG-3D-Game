using UnityEngine;
using UnityEngine.AI; // Required for the NavMeshAgent
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    [Header("Setup")]
    public NavMeshAgent agent;
    public Transform player;
    public List<Transform> waypoints;

    [Header("Settings")]
    public float detectionRange = 10f; // How far the enemy can "see"
    public float chaseSpeed = 5f;
    public float patrolSpeed = 3f;

    // State Machine
    private enum State { Patrolling, Chasing }
    private State currentState;
    private int currentWaypointIndex = 0;

    void Start()
    {
        // Initialize state
        currentState = State.Patrolling;
        agent.speed = patrolSpeed;

        // Move to first waypoint immediately
        if (waypoints.Count > 0)
        {
            agent.SetDestination(waypoints[0].position);
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                PatrolBehavior();
                break;
            case State.Chasing:
                ChaseBehavior();
                break;
        }
    }

    void PatrolBehavior()
    {
        agent.speed = patrolSpeed;

        // 1. Check if we are close to the current waypoint
        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            // Move to next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }

        // 2. Check for Player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < detectionRange)
        {
            currentState = State.Chasing;
        }
    }

    void ChaseBehavior()
    {
        agent.speed = chaseSpeed;

        // 1. Move towards player
        agent.SetDestination(player.position);

        // 2. Check if player escaped
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange * 1.5f) // * 1.5f adds a buffer so it doesn't flicker
        {
            // Go back to patrol
            currentState = State.Patrolling;
            // Resume moving to the last known waypoint
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    // Visual Aid for debugging in the Scene View
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}