using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// The opacity of the blood overlay increases as the player takes more damage. Y'know, like in every shooter ever made?
/// </summary>
public class HealthOverlay : MonoBehaviour
{
    public HealthSystem health; // The amount of health the player has left.
    private RawImage image; // The current opacity of the image.

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<RawImage>(); // Image reference is collected.
    }

    // Update is called once per frame
    void Update()
    {
        image.color = Color.white * (3 - health.health) / 3; // The opacity of the blood overlay increases as the player takes more damage. Y'know, like in every shooter ever made?
    }
}
