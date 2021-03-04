using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public PlayerMovement moveScript; // Referenced so that there is a target for the camera to orbit.
    private PlayerTargetingScript targetingScript; // The status of the player's lock-on is referenced to change the camera accordingly.
    private Camera cam; // The camera that is attached to the rig.

    private float yaw = 0;
    private float pitch = 0;

    public float cameraSensitivityX = 1; // How quickly the player can move the camera horizontally.
    public float cameraSensitivityY = 1; // How quickly the player can move the camera vertically.

    public float shakeIntensity = 0; // The current amount the camera is shaking.

    // Start is called before the first frame update
    private void Start() // Get targeting and cam scripts.
    {
        targetingScript = moveScript.GetComponent<PlayerTargetingScript>();
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update() // Cycles through each of this scripts functions.
    {
        PlayerOrbitCamera();

        transform.position = moveScript.transform.position;

        // if aiming, set camera's rotation to look at target
        RotateCamToLookAtTarget();

        // "zoom" in the camera
        ZoomCamera();

        ShakeCamera();
    }

    public void Shake(float intensity = 1)
    {
        shakeIntensity = intensity;
    }

    private void ShakeCamera()
    {
        if (shakeIntensity < 0) shakeIntensity = 0;

        if (shakeIntensity > 0) shakeIntensity -= Time.deltaTime;
        else return; // shake intentisty is 0, so do nothing...

        // pick a SMALL random rotation:
        Quaternion targetRot = AnimMath.Lerp(Random.rotation, Quaternion.identity, .99f);

        // cam.transform.localRotation *= targetRot
        cam.transform.localRotation = AnimMath.Lerp(cam.transform.localRotation, cam.transform.localRotation * targetRot, shakeIntensity * shakeIntensity);
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

    private void PlayerOrbitCamera() // Rotates the camera around the player based on user input.
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
