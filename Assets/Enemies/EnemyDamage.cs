using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damageToDeal = 15;
    public float attackCooldown = 1.0f; // Seconds between hits
    private float lastAttackTime;
    private EnemyAI enemyAI;
    // This runs when the Enemy touches something physically


    private void OnTriggerEnter(Collider other)
    {
        TryDealDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDealDamage(other);
    }

    private void TryDealDamage(Collider other)
    {
        // Only damage the Player
        if (!other.CompareTag("Player"))
            return;

        // Enforce hit cooldown
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        // Damage the player
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damageToDeal);
            lastAttackTime = Time.time;

            Debug.Log($"Enemy hit player for {damageToDeal} damage");
        }
    }


    //private void Start()
    //{
    //    enemyAI = GetComponentInParent<EnemyAI>();
    //}
    //void OnTriggerStay(Collider collision)
    //{
    //    // 1. Check if we touched the Player
    //    if (collision.gameObject.CompareTag("Player"))
    //    {

    //        // 2. Check Cooldown (Don't hit 60 times a second)
    //        if (Time.time > lastAttackTime + attackCooldown)
    //        {
    //            AttackPlayer(collision.gameObject);
    //            lastAttackTime = Time.time;
    //        }
    //    }
    //}

    //void AttackPlayer(GameObject player)
    //{
    //    // PlayerHealth for player
    //    PlayerHealth health = player.GetComponent<PlayerHealth>();
    //    health.TakeDamage(damageToDeal);



    //    // For now, we just print to console. 
    //    // Later, you will call: player.GetComponent<PlayerHealth>().TakeDamage(10);
    //    Debug.Log("Ouch! Player hit for " + damageToDeal + " damage!");
    //}
}