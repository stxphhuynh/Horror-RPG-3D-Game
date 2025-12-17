using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackEvents : MonoBehaviour
{
    [Header("Reference to the hitbox damage script")]
    public EnemyDamage hitboxDamage;

    // These are called by Animation Events on the Animator's GameObject
    public void OpenDamageWindow()
    {
        if (hitboxDamage != null)
        {
            hitboxDamage.OpenDamageWindow();
        }
    }

    public void CloseDamageWindow()
    {
        if (hitboxDamage != null)
        {
            hitboxDamage.CloseDamageWindow();
        }
    }
}

