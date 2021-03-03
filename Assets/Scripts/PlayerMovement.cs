using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{
    public HealthSystem health;
    public Camera cam;
    private CharacterController pawn;
    public float walkSpeed = 5;
    
    public Rigidbody headLimb;
    public Rigidbody torsoLimb;
    public Rigidbody armLimb;
    public Rigidbody legLimb;

    public GameObject head;
    public GameObject torso;
    public GameObject arm1;
    public GameObject arm2;
    public GameObject leg1;
    public GameObject leg2;

    public Transform legBone1;
    public Transform legBone2;

    private Vector3 inputDirection = new Vector3();

    private float timeLeftGrounded = 0;

    private float verticalVelocity = 0;

    private bool isJumpHeld = false;

    private float jumpImpulse = 10;

    private bool gibbed = false;

    public ParticleSystem blood;

    public bool isGrounded
    {
        get
        { // return true is pawn is on ground OR "coyote time"
            return pawn.isGrounded || timeLeftGrounded > 0;
        }
    }

    void Start()
    {
        cam = Camera.main;
        pawn = GetComponent<CharacterController>();
        health = GetComponent<HealthSystem>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (health.dead)
        {
            if (gibbed)
            {
                return;
            }
            else
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
            }
        }

        else
        {
            // countdown
            if (timeLeftGrounded > 0) timeLeftGrounded -= Time.deltaTime;

            MovePlayer();
            if (isGrounded) WiggleLegs();
            else AirLegs(); // jump / falling
        }
        print(isGrounded);
    }

    private void WiggleLegs()
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

    private void AirLegs()
    {
        legBone1.localRotation = AnimMath.Slide(legBone1.localRotation, Quaternion.Euler(30, 0, 0));
        legBone2.localRotation = AnimMath.Slide(legBone2.localRotation, Quaternion.Euler(-30, 0, 0));
    }

    private void MovePlayer()
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
