using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damageToDeal = 15;
    public float attackCooldown = 1.0f; // Seconds between hits
    private float lastAttackTime;

    public bool damageWindowOpen = false;
    public bool hasHitAttack = false;

  

    // called when something enters trigger
    private void OnTriggerEnter(Collider other)
    {
        TryDealDamage(other);
    }

    // called when frame stays in trigger
    private void OnTriggerStay(Collider other)
    {
        TryDealDamage(other);
    }

  
    private void TryDealDamage(Collider other)
    {
        // Only damage the Player
        if (!other.CompareTag("Player"))
            return;
        // damage during window of attack
        if (!damageWindowOpen)
        {
            return;
        }

        if (hasHitAttack)
        {
            return;
        }

        // Enforce hit cooldown
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        // Damage the player
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damageToDeal);

            Debug.Log($"Enemy hit player for {damageToDeal} damage");
        }

        lastAttackTime = Time.time;
        hasHitAttack = true;
    }


    // functions called by animation events
    public void OpenDamageWindow()
    {
        damageWindowOpen = true;
        hasHitAttack = false;
        Debug.Log("Damage window OPEN");
    }

    // Call this shortly after the impact frame
    public void CloseDamageWindow()
    {
        damageWindowOpen = false;
        Debug.Log("Damage window CLOSED");
    }

}