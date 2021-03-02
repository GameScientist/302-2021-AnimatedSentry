using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryTurret : MonoBehaviour
{
    public string state;
    public Transform rotator;
    public Transform leg1;
    public Transform leg2;
    public Transform leg3;
    public Transform leg4;

    public Transform target;

    private float timeActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        print(state);
        //cooldownScan -= Time.deltaTime;
        //if (cooldownScan <= 0 || target == null) ScanForTargets();
            switch (state)
        {
            case "Idle":
                rotator.rotation = AnimMath.Slide(rotator.rotation, Quaternion.Euler(0, 0, 0));
                transform.position = (transform.position.x * transform.right) + (transform.up * AnimMath.Slide(transform.position.y, 0.75f, 0.1f)) + (transform.position.z * transform.forward);
                leg1.localPosition = (transform.localPosition.x * transform.right) + transform.up * AnimMath.Slide(leg1.localPosition.y, 0, 0.1f) + (transform.localPosition.z * transform.forward);
                leg2.localPosition = (transform.localPosition.x * transform.right) + transform.up * AnimMath.Slide(leg2.localPosition.y, 0, 0.1f) + (transform.localPosition.z * transform.forward);
                leg3.localPosition = (transform.localPosition.x * transform.right) + transform.up * AnimMath.Slide(leg3.localPosition.y, 0, 0.1f) + (transform.localPosition.z * transform.forward);
                leg4.localPosition = (transform.localPosition.x * transform.right) + transform.up * AnimMath.Slide(leg4.localPosition.y, 0, 0.1f) + (transform.localPosition.z * transform.forward);
                break;
            case "Walk":
                transform.position = (transform.position.x * transform.right) + (transform.up * AnimMath.Slide(transform.position.y, 1.25f, 0.1f)) + (transform.position.z * transform.forward);
                leg1.localPosition = (0.25f * transform.right) - (transform.up * ((Mathf.Sin(Time.time*25)+1)/2)) + (0.25f* transform.forward);
                leg2.localPosition = (-0.25f * transform.right) - (transform.up * ((Mathf.Cos(Time.time*25)+1)/2)) + (0.25f * transform.forward);
                leg3.localPosition = (0.25f * transform.right) - (transform.up * ((Mathf.Sin(Time.time*25 + 1)+1)/2)) + (-0.25f * transform.forward);
                leg4.localPosition = (-0.25f * transform.right) - (transform.up * ((Mathf.Cos(Time.time*25 + 1)+1)/2)) + (-0.25f * transform.forward);
                break;
            case "Fire":
                break;
            case "Destroyed":
                break;
        }
    }
}
