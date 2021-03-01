using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingScript : MonoBehaviour
{
    public Transform target;
    public bool wantsToTarget = false;
    public bool wantsToAttack = false;
    public float visionDistance = 10;
    public float visionAngle = 45;
    private List<TargetObject> potentialTargets = new List<TargetObject>();
    float cooldownScan = 0;
    float cooldownPick = 0;

    float cooldownShoot = 0;

    public float roundsPerSecond = 10;

    // references to the player's arm "bones":
    public Transform armL;
    public Transform armR;

    private Vector3 startPosArmL;
    private Vector3 startPosArmR;

    public ParticleSystem prefabMuzzleFlash;

    public Transform handL;
    public Transform handR;

    public CameraOrbit camOrbit;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        startPosArmL = armL.localPosition;
        startPosArmR = armR.localPosition;

        camOrbit = Camera.main.GetComponentInParent<CameraOrbit>();
    }

    // Update is called once per frame
    void Update()
    {
        wantsToTarget = Input.GetButton("Fire2");
        wantsToAttack = Input.GetButton("Fire1");

        if (!wantsToTarget) target = null;

        cooldownScan -= Time.deltaTime;
        if (cooldownScan <= 0 || (target == null && wantsToTarget)) ScanForTargets();

        cooldownPick -= Time.deltaTime;
        if (cooldownPick <= 0) PickATarget();

        if (cooldownShoot > 0) cooldownShoot -= Time.deltaTime;

        if (target && !CanSeeThing(target)) target = null;

        SlideArmsHome();

        DoAttack();
    }

    private void SlideArmsHome()
    {
        armL.localPosition = AnimMath.Slide(armL.localPosition, startPosArmL, .01f);
        armR.localPosition = AnimMath.Slide(armR.localPosition, startPosArmR, .01f);
    }
    
    private void DoAttack()
    {
        if (cooldownShoot > 0) return; 
        if (!wantsToTarget) return; // player not targeting
        if (!wantsToAttack) return; // player not shooting
        if (target == null) return;
        if (!CanSeeThing(target)) return;

        HealthSystem targetHealth = target.GetComponent<HealthSystem>();
        if (targetHealth)
        {
            targetHealth.TakeDamage();
        }

        print("PEW");
        cooldownShoot = 1 / roundsPerSecond;

        // attack!
        camOrbit.Shake(.5f);

        if(handL) Instantiate(prefabMuzzleFlash, handL.position, handL.rotation);
        if(handR) Instantiate(prefabMuzzleFlash, handR.position, handR.rotation);

        // trigger arm animation

        // rotates arms up:
        armL.localEulerAngles += new Vector3(-20, 0, 0);
        armR.localEulerAngles += new Vector3(-20, 0, 0);

        // moves the arms backwards:
        armL.position += -armL.transform.forward * .1f;
        armR.position += -armR.transform.forward * .1f;
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
