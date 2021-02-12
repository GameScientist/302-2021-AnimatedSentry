using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingScript : MonoBehaviour
{
    public Transform target;
    public bool wantsToTarget = false;
    public float visionDistance = 10;
    private List<TargetObject> potentialTargets = new List<TargetObject>();
    float cooldownScan = 0;
    float cooldownPick = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        wantsToTarget = Input.GetButton("Fire2");

        cooldownScan -= Time.deltaTime;
        if (cooldownScan <= 0) ScanForTargets();

        cooldownPick -= Time.deltaTime;
        if (cooldownPick <= 0) PickATarget();
    }

    private void ScanForTargets()
    {
        cooldownScan = 1; // do the next scan in 2 seconds.
        TargetObject[] things = GameObject.FindObjectsOfType<TargetObject>();

        //empty list
        potentialTargets.Clear();

        //refill the list
        foreach(TargetObject thing in things)
        {
            // check how far away thing is
            Vector3 disToThing = thing.transform.position - transform.position;

            if(disToThing.sqrMagnitude < visionDistance * visionDistance)
            {
                if (Vector3.Angle(transform.forward, disToThing) < 45)
                {
                    potentialTargets.Add(thing);
                }
            }

            // check what direction it is in
        }
    }

    void PickATarget()
    {
        cooldownPick = 0.25f;

        if (target != null) return; // we already have a target...

        float closestDistanceSoFar = 0;

        foreach(var pt in potentialTargets)
        {
            float dd = (pt.transform.position - transform.position).sqrMagnitude;

            if(dd < closestDistanceSoFar || target == null)
            {
                target = pt.transform;
                closestDistanceSoFar = dd;
            }
        }
    }
}
