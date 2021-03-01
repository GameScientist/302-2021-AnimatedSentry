using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float health { get; private set; }
    public float healthMax = 100;

    public bool dead = false;

    private void Start()
    {
        health = healthMax;
    }

    public void TakeDamage()
    {
        health--;

        if (health <= 0) dead = true;
    }

    public void Die()
    {
        // removes this gameobject from the game:
        Destroy(gameObject);
    }
}
