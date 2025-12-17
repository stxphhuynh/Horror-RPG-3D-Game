using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damageToDeal = 10;
    public float attackCooldown = 1.0f; // Seconds between hits
    private float lastAttackTime;
    private EnemyAI enemyAI;
    // This runs when the Enemy touches something physically


    private void Start()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }
    void OnTriggerStay(Collider collision)
    {
        // 1. Check if we touched the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            //if (enemyAI == null || !enemyAI.IsAttackingState) return;
            // 2. Check Cooldown (Don't hit 60 times a second)
            if (Time.time > lastAttackTime + attackCooldown)
            {
                AttackPlayer(collision.gameObject);
                lastAttackTime = Time.time;
            }
        }
    }

    void AttackPlayer(GameObject player)
    {
        // PlayerHealth for player
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        health.TakeDamage(damageToDeal);



        // For now, we just print to console. 
        // Later, you will call: player.GetComponent<PlayerHealth>().TakeDamage(10);
        Debug.Log("Ouch! Player hit for " + damageToDeal + " damage!");
    }
}