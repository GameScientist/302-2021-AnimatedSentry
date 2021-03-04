using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// When the player enters this sphere, the turret will stop firing at the player and try to get within a better distance.
/// </summary>
public class FireExitSphere : MonoBehaviour
{
    private SentryTurret sentry;
    // Start is called before the first frame update
    void Start()
    {
        sentry = GetComponentInParent<SentryTurret>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) sentry.state = "Walk";
    }
}