using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Call this function from your Weapon script later
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log(transform.name + " took " + damageAmount + " damage!");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(transform.name + " has died.");
        // Optional: Spawn loot here
        // Optional: Play death sound
        Destroy(gameObject); // Removes the enemy from the game
    }

    // DEBUGGING TOOL: This lets you click the enemy to test damage
    void OnMouseDown()
    {
        TakeDamage(20);
    }
}