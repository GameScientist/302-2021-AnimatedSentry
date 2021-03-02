using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkEnterSphere : MonoBehaviour
{
    private SentryTurret sentry;
    // Start is called before the first frame update
    void Start()
    {
        sentry = GetComponentInParent<SentryTurret>();
        print(sentry);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) sentry.state = "Walk";
    }
}
