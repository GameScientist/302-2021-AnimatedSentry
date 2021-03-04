using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Upon entry, the turret will notice the player and move towards it to attack it.
/// </summary>
public class WalkEnterSphere : MonoBehaviour
{
    private SentryTurret sentry;
    // Start is called before the first frame update
    void Start()
    {
        sentry = GetComponentInParent<SentryTurret>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) sentry.state = "Walk";
    }
}
