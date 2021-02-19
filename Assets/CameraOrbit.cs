using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public PlayerMovement moveScript;
    private PlayerTargetingScript targetingScript;
    private Camera cam;

    private float yaw = 0;
    private float pitch = 0;

    public float cameraSensitivityX = 1;
    public float cameraSensitivityY = 1;

    // Start is called before the first frame update
    private void Start()
    {
        targetingScript = moveScript.GetComponent<PlayerTargetingScript>();
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerOrbitCamera();

        transform.position = moveScript.transform.position;

        // if aiming, set camera's rotation to look at target
        RotateCamToLookAtTarget();

        // "zoom" in the camera
        ZoomCamera();
    }

    private void ZoomCamera()
    {
        float dis = 10;
        if (IsTargeting()) dis = 3;

        cam.transform.localPosition = AnimMath.Slide(cam.transform.localPosition, new Vector3(0, 0, -dis), .001f);
    }

    private bool IsTargeting()
    {
        return (targetingScript && targetingScript.target != null && targetingScript.wantsToTarget);
    }

    private void RotateCamToLookAtTarget()
    {
        // if targeting, set rotation to look at target
        if (targetingScript && targetingScript.target != null)
        {
            Vector3 vToTarget = targetingScript.target.position - cam.transform.position;
            Quaternion targetRot = Quaternion.LookRotation(vToTarget, Vector3.up);

            cam.transform.rotation = AnimMath.Slide(cam.transform.rotation, targetRot, .001f);
        }
        else
        {
            // if NOT target, reset rotation
            cam.transform.localRotation = AnimMath.Slide(cam.transform.localRotation, Quaternion.identity, .001f);
        }
    }

    private void PlayerOrbitCamera()
    {
        float mx = Input.GetAxisRaw("Mouse X");
        float my = Input.GetAxisRaw("Mouse Y");

        yaw += mx * cameraSensitivityX;
        pitch += my * cameraSensitivityY;

        if (IsTargeting()) // z-targeting / lock-on
        {
            pitch = Mathf.Clamp(pitch, -5, 5);
            float playerYaw = moveScript.transform.eulerAngles.y;
            // clamp camera rig yaw to player yaw +- 30
            yaw = Mathf.Clamp(yaw, playerYaw-40, playerYaw+40);
        }
        else
        {// not targeting / free look
            pitch = Mathf.Clamp(pitch, -5, 89);
        }

        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
    }
}
