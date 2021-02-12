using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera cam;
    private CharacterController pawn;
    public float walkSpeed = 5;

    void Start()
    {
        cam = Camera.main;
        pawn = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
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

        Vector3 inputDirection = transform.forward * v + transform.right * h;

        pawn.SimpleMove(inputDirection * walkSpeed);
    }
}