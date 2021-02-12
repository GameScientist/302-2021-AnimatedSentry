using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAt : MonoBehaviour
{

    public Transform target;
    private PlayerTargetingScript playerTargeting;

    public Vector3 naturalAimDirection;

    private Quaternion startingRotation;

    public bool lockRotationX;
    public bool lockRotationY;
    public bool lockRotationZ;

    // Start is called before the first frame update
    void Start()
    {
        startingRotation = transform.localRotation;
        playerTargeting = GetComponentInParent<PlayerTargetingScript>();
    }

    // Update is called once per frame
    void Update()
    {
        TurnTowardsTarget();
    }

    private void TurnTowardsTarget()
    {
        if (playerTargeting && playerTargeting.target)
        {
            Vector3 disToTarget = playerTargeting.target.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(disToTarget, Vector3.up);

            Vector3 euler1 = transform.localEulerAngles; // get local angle BEFORE rotation
            Quaternion prevRot = transform.rotation;
            transform.rotation = targetRotation; // set rotation
            Vector3 euler2 = transform.localEulerAngles; // get local angles AFTER rotation

            if (lockRotationX) euler2.x = euler1.x; // revert x to previous value
            if (lockRotationY) euler2.y = euler1.y; // revert x to previous value
            if (lockRotationZ) euler2.z = euler1.z; // revert x to previous value

            transform.rotation = prevRot; // revert rotation

            // animate rotation
            transform.localRotation = AnimMath.Slide(transform.localRotation, Quaternion.Euler(euler2), .01f);
        }
        else
        {
            // figure out bone rotation with no target:
            Quaternion targetRotation = Quaternion.LookRotation(naturalAimDirection, Vector3.up);
            transform.localRotation = AnimMath.Slide(transform.localRotation, startingRotation, .1f);
        }
    }
}
