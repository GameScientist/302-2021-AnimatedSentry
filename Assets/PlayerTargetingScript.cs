using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingScript : MonoBehaviour
{
    public Transform target;
    public bool wantsToTarget = false;
    public float visionDistance = 10;
    public float visionAngle = 45;
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

        if (!wantsToTarget) target = null;

        cooldownScan -= Time.deltaTime;
        if (cooldownScan <= 0 || (target == null && wantsToTarget)) ScanForTargets();

        cooldownPick -= Time.deltaTime;
        if (cooldownPick <= 0) PickATarget();

        if (target && !CanSeeThing(target)) target = null;
    }
    
    private bool CanSeeThing(Transform thing)
    {
        if (!thing) return false; // uh... error

        Vector3 vToThing = thing.position - transform.position;

        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false; // too far away to see...

        if(Vector3.Angle(transform.forward, vToThing) > visionAngle) return false; // out of vision "cone"

        // TODO: check occlusion

        return true;
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
            if (CanSeeThing(thing.transform)) potentialTargets.Add(thing);

            // check what direction it is in
        }
    }

    void PickATarget()
    {
        cooldownPick = 0.25f;

        target = null; // we already have a target...

        float closestDistanceSoFar = 0;

        foreach(TargetObject pt in potentialTargets)
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
