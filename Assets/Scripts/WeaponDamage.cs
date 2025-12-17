using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public int damage = 10;
    public bool canDealDamage = false;

    // to avoid hitting multiple times in one swing
    private bool hasHitThisSwing = false;

    // Audio
    public AudioSource swing;
    public AudioSource hit;

    // knockback from weapon
    public float knockbackForce = 5f;
    public float knockbackDuration = .2f;


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
        
        if (!canDealDamage || hasHitThisSwing)
            return;

        // deal damage to enemy
        EnemyStats enemy = other.GetComponentInParent<EnemyStats>();
        if (enemy != null)
        {
            Debug.Log("HIT " + other.name);
            enemy.TakeDamage(damage);
            hasHitThisSwing = true; // only one hit per swing
            hit.Play();
        }
        // knockback to enemy
        EnemyAI enemyAI = other.GetComponentInParent<EnemyAI>();
        if (enemyAI != null) {
            Debug.Log("KNOCKBACK " + enemyAI.name);
            enemyAI.ApplyKnockBack(transform.position, knockbackForce, knockbackDuration);
        
        }
    }

    // called by player when starting a new swing
    public void ResetSwing()
    {
        hasHitThisSwing = false;
    }

    public void PlaySwing()
    {
        swing.Play();
    }

}

