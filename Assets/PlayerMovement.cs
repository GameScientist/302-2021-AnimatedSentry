using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera cam;
    private CharacterController pawn;
    public float walkSpeed = 5;

    public Transform leg1;
    public Transform leg2;

    private Vector3 inputDirection = new Vector3();

    private float timeLeftGrounded = 0;

    private float verticalVelocity = 0;

    private bool isJumpHeld = false;

    public float jumpImpulse = 20;

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
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // countdown
        if (timeLeftGrounded > 0) timeLeftGrounded -= Time.deltaTime;

        MovePlayer();
        if (pawn.isGrounded) WiggleLegs();
        else AirLegs(); // jump / falling
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

        leg1.localRotation = AnimMath.Slide(leg1.localRotation, Quaternion.AngleAxis(wave, axis), .001f);
        leg2.localRotation = AnimMath.Slide(leg2.localRotation, Quaternion.AngleAxis(-wave, axis), .001f);
    }

    private void AirLegs()
    {
        leg1.localRotation = AnimMath.Slide(Quaternion.Euler(0, 0, 0), Quaternion.Euler(30, 0, 0));
        leg1.localRotation = AnimMath.Slide(Quaternion.Euler(0, 0, 0), Quaternion.Euler(-30, 0, 0));
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

        // apply gravity:
        verticalVelocity += 10 * Time.deltaTime;

        // adds lateral movement to vertical movement
        Vector3 moveDelta = inputDirection * walkSpeed + verticalVelocity * Vector3.down;

        CollisionFlags flags = pawn.Move(moveDelta * Time.deltaTime); // 0, -1, 0

        if (pawn.isGrounded)
        {
            verticalVelocity = 0; // on ground, zero-out gravity below.
            timeLeftGrounded = 0.16f;
        }

        if(isGrounded)
        {

            if (isJumpHeld)
            {
                verticalVelocity = -jumpImpulse;
                timeLeftGrounded = 0; // not on ground (for animation's sake)
            }
        }
    }
}
