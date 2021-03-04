using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows the player to lock-on to and fire at enemies.
/// </summary>
public class PlayerTargetingScript : MonoBehaviour
{
    public AudioManager audioManager; // Stores all of the sound produced by player gunfire.
    public Transform target; // The enemy the player is currently targetting.
    public bool wantsToTarget = false; // Tracks if the player is trying lock on to an enemy.
    public bool wantsToAttack = false; // Tracks if the player is trying to fire at an enemy.
    public float visionDistance = 10; // How far the player can lock on to an enemy from.
    public float visionAngle = 45; // Controls the cone from the camera in which the player can select a target from.
    private List<TargetObject> potentialTargets = new List<TargetObject>(); // The list of all of the objects in the area the player can target.
    float cooldownScan = 0; // How long the player has to wait before scanning for a new target.
    float cooldownPick = 0; // How long the player has to wait before picking a new target.

    float cooldownShoot = 0; // How long the player has to wait in between shots.

    public float roundsPerSecond = 10; // How many times the player can fire their weapon per second.

    // references to the player's arm "bones":
    public Transform armL;
    public Transform armR;

    // References the position the arms must always return to.
    private Vector3 startPosArmL;
    private Vector3 startPosArmR;

    public ParticleSystem prefabMuzzleFlash; // The particle system that emits from the gun when firing.

    // Where the particle system is emitted from.
    public Transform handL;
    public Transform handR;

    public CameraOrbit camOrbit; // References the camera that must be shaked when the player fires their gun.

    // Start is called before the first frame update
    void Start() // Sets the arm start positions and gets the camera orbit.
    {
        Cursor.lockState = CursorLockMode.Locked;

        startPosArmL = armL.localPosition;
        startPosArmR = armR.localPosition;

        camOrbit = Camera.main.GetComponentInParent<CameraOrbit>();
    }

    // Update is called once per frame
    void Update() // Registers player input.
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

    private void SlideArmsHome() // Returns arms to their start positions after moving.
    {
        armL.localPosition = AnimMath.Slide(armL.localPosition, startPosArmL, .01f);
        armR.localPosition = AnimMath.Slide(armR.localPosition, startPosArmR, .01f);
    }
    
    private void DoAttack() // Fires the player's weapons, damaging whatever has been locked onto.
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

        audioManager.Play("Gun Fire");
        audioManager.Play("Sentry Turret Impact");
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

    private bool CanSeeThing(Transform thing) // Ddecides whether a target is within the player's line of sight.
    {
        if (!thing) return false; // uh... error

        Vector3 vToThing = thing.position - transform.position;

        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false; // too far away to see...

        if(Vector3.Angle(transform.forward, vToThing) > visionAngle) return false; // out of vision "cone"

        // TODO: check occlusion

        return true;
    }

    private void ScanForTargets() // Compiles a list of targets based on the targets the player can see.
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

    void PickATarget() // Locks on to the closest enemy in the player's line of sight.
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
