using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Keeps track of how many hit points the player has left until they die.
/// </summary>
public class HealthSystem : MonoBehaviour
{

    public float health { get; private set; } // How many hit points the player has.
    private float healthMax = 3; // The amount of health the player starts with.

    public bool dead = false; // Is true when the character runs out of health.

    private void Start()
    {
        health = healthMax; // Each character starts with full health.
    }

    public void TakeDamage() // Each time a character is shot, they lose one hit point and die if they have no hit points left.
    {
        health--;

        if (health <= 0)
        {
            dead = true;
        }
    }
}
