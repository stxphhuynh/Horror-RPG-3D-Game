using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public int damage = 10;
    public bool canDealDamage = false;

    // to avoid hitting multiple times in one swing
    private bool hasHitThisSwing = false;

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

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            Debug.Log("HIT " + other.name);
            enemy.TakeDamage(damage);
            hasHitThisSwing = true; // only one hit per swing
        }
    }

    // called by player when starting a new swing
    public void ResetSwing()
    {
        hasHitThisSwing = false;
    }
}

