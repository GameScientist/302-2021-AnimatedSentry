using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
/// <summary>
/// Tracks player movement based on input and gibbing.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public AudioManager audioManager; // Keeps track of the sound effects the player produces.

    public HealthSystem health; // References how much health the player has until death occurs.
    public Camera cam; // References the camera that follows the player.
    private CharacterController pawn; // References the character controller compoent so that the player character can be moved.
    public float walkSpeed = 5; // Controls how fast the player character walks.
    
    /// <summary>
    /// Body parts are shot out of the character upon death.
    /// </summary>
    public Rigidbody headLimb;
    public Rigidbody torsoLimb;
    public Rigidbody armLimb;
    public Rigidbody legLimb;

    /// <summary>
    /// The location of each body part is stored so objects can be spawned at their location and destroyed upon death.
    /// </summary>
    public GameObject head;
    public GameObject torso;
    public GameObject arm1;
    public GameObject arm2;
    public GameObject leg1;
    public GameObject leg2;

    /// <summary>
    /// The legs are referenced to change their rotation based on the player's current action.
    /// </summary>
    public Transform legBone1;
    public Transform legBone2;

    private Vector3 inputDirection = new Vector3(); // The direction the player is traveling in relation to the camera.

    private float timeLeftGrounded = 0; // Used to understand is the player is on the ground or how much time they have to jump before falling after walking off a ledge.

    private float verticalVelocity = 0; // The current amount of vertical velocity the player has.

    private bool isJumpHeld = false; // If the player is still pressing down on the jump button.

    private float jumpImpulse = 10; // How much vertical velocity is added to the player when the player jumps.

    private bool gibbed = false; // If the player has already gibbed or not. Used to avoid repeated gibbing.

    public ParticleSystem blood; // Spawns from the player among death.

    public bool isGrounded
    {
        get
        { // return true is pawn is on ground OR "coyote time"
            return pawn.isGrounded || timeLeftGrounded > 0;
        }
    }

    void Start() // Components are gathered.
    {
        cam = Camera.main;
        pawn = GetComponent<CharacterController>();
        health = GetComponent<HealthSystem>();
        Cursor.lockState = CursorLockMode.Locked; // The mouse will not appear during gameplay.
        audioManager.Play("Buzzer"); // A buzzer is played to give the impression that something is starting.
    }

    // Update is called once per frame
    void Update()
    {
        if (health.dead)
        {
            if (gibbed) // The player has already gibbed, so there is no need for the player to gib again.
            {
                return;
            }
            else // The player gibs.
            {
                Rigidbody headGib;
                headGib = Instantiate(headLimb, head.transform);
                headGib.transform.SetParent(null);
                headGib.velocity = transform.TransformDirection(transform.up * 10 + transform.forward * 10);
                Rigidbody torsoGib;
                torsoGib = Instantiate(torsoLimb, torso.transform);
                torsoGib.transform.SetParent(null);
                torsoGib.velocity = transform.TransformDirection((transform.up * Random.Range(-1f, 1f)) + (transform.right * Random.Range(-1f, 1f)) + (transform.forward * Random.Range(-1f, 1f)));
                Rigidbody arm1Gib;
                arm1Gib = Instantiate(armLimb, arm1.transform);
                arm1Gib.transform.SetParent(null);
                arm1Gib.velocity = transform.TransformDirection((transform.up * Random.Range(-1f, 1f)) + (transform.right * Random.Range(-1f, 1f)) + (transform.forward * Random.Range(-1f, 1f)));
                Rigidbody arm2Gib;
                arm2Gib = Instantiate(armLimb, arm2.transform);
                arm2Gib.transform.SetParent(null);
                arm2Gib.velocity = transform.TransformDirection((transform.up * Random.Range(-1f, 1f)) + (transform.right * Random.Range(-1f, 1f)) + (transform.forward * Random.Range(-1f, 1f)));
                Rigidbody leg1Gib;
                leg1Gib = Instantiate(legLimb, leg1.transform);
                leg1Gib.transform.SetParent(null);
                leg1Gib.velocity = transform.TransformDirection((transform.up * Random.Range(-1f, 1f)) + (transform.right * Random.Range(-1f, 1f)) + (transform.forward * Random.Range(-1f, 1f)));
                Rigidbody leg2Gib;
                leg2Gib = Instantiate(legLimb, leg2.transform);
                leg2Gib.transform.SetParent(null);
                leg2Gib.velocity = transform.TransformDirection((transform.up * Random.Range(-1f, 1f)) + (transform.right * Random.Range(-1f, 1f)) + (transform.forward * Random.Range(-1f, 1f)));
                gibbed = true;
                Destroy(head);
                Destroy(torso);
                Destroy(arm1);
                Destroy(arm2);
                Destroy(leg1);
                Destroy(leg2);
                PlayerTargetingScript targeting = GetComponent<PlayerTargetingScript>();
                Destroy(targeting);
                Instantiate(blood, transform);
                audioManager.Play("Player Death");
            }
        }

        else // The player's position is updated using three functions.
        {
            // countdown
            if (timeLeftGrounded > 0) timeLeftGrounded -= Time.deltaTime;

            MovePlayer();
            if (isGrounded) WiggleLegs();
            else AirLegs(); // jump / falling
        }
    }

    private void WiggleLegs() // The legs will rotate based on the direction the player is traveling in.
    {
        float degrees = 45;

        float speed = 10;


        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDirection);
        Vector3 axis = Vector3.Cross(inputDirLocal, Vector3.up);

        // check the alignment of inputDirLocal against forward vector

        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);

        //if (alignment < 0) alignment *= -1; // flips negative numbers

        alignment = Mathf.Abs(alignment); // flips negative numbers

        degrees *= AnimMath.Lerp(0.25f, 1, alignment); // decrease 'degrees' when strafing

        float wave = Mathf.Sin(Time.time * speed) * degrees;
        // 1 = yes!
        // 0 = no!
        // -1 = yes!

        legBone1.localRotation = AnimMath.Slide(legBone1.localRotation, Quaternion.AngleAxis(wave, axis), .001f);
        legBone2.localRotation = AnimMath.Slide(legBone2.localRotation, Quaternion.AngleAxis(-wave, axis), .001f);
    }

    private void AirLegs() // The player's legs are supsended in mid-air.
    {
        legBone1.localRotation = AnimMath.Slide(legBone1.localRotation, Quaternion.Euler(30, 0, 0));
        legBone2.localRotation = AnimMath.Slide(legBone2.localRotation, Quaternion.Euler(-30, 0, 0));
    }

    private void MovePlayer() // The player's position is updated based on player input and jumping status.
    {
        float h = Input.GetAxis("Horizontal"); // Strafing
        float v = Input.GetAxis("Vertical"); // forward / backward

        bool isTryingToMove = (h != 0 || v != 0);
        if (isTryingToMove)
        {
            // turn to face the correct direction...
            float camYaw = cam.transform.eulerAngles.y;
            transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(0, camYaw, 0), 0.02f);
        }

        inputDirection = transform.forward * v + transform.right * h;

        if (inputDirection.sqrMagnitude > 1) inputDirection.Normalize();

        if (Input.GetButtonDown("Jump")) isJumpHeld = true;
        else isJumpHeld = false;

        // apply gravity:
        verticalVelocity += 10 * Time.deltaTime;

        // adds lateral movement to vertical movement
        Vector3 moveDelta = inputDirection * walkSpeed + verticalVelocity * Vector3.down;

        CollisionFlags flags = pawn.Move(moveDelta * Time.deltaTime); // 0, -1, 0


        if(isGrounded)
        {
            verticalVelocity = 0; // on ground, zero-out gravity below.
            timeLeftGrounded = 0.16f;

            if (isJumpHeld)
            {
                verticalVelocity = -jumpImpulse;
                timeLeftGrounded = 0; // not on ground (for animation's sake)
            }
        }
    }
}
