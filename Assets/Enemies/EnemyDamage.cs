using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damageToDeal = 10;
    public float attackCooldown = 1.0f; // Seconds between hits
    private float lastAttackTime;

    // This runs when the Enemy touches something physically
    void OnCollisionStay(Collision collision)
    {
        // 1. Check if we touched the Player
        if (collision.gameObject.CompareTag("Player"))
        {
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
        // For now, we just print to console. 
        // Later, you will call: player.GetComponent<PlayerHealth>().TakeDamage(10);
        Debug.Log("Ouch! Player hit for " + damageToDeal + " damage!");
    }
}