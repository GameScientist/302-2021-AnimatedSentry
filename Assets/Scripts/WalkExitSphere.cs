using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkExitSphere : MonoBehaviour
{
    private SentryTurret sentry;
    // Start is called before the first frame update
    void Start()
    {
        sentry = GetComponentInParent<SentryTurret>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) sentry.state = "Idle";
    }
}
