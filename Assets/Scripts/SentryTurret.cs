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

    public Transform player;

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
                rotator.rotation = AnimMath.Slide(rotator.rotation, Quaternion.Euler(270, 0, 0));
                transform.position = (transform.position.x * transform.right) + (transform.up * AnimMath.Slide(transform.position.y, 0.75f, 0.1f)) + (transform.position.z * transform.forward);
                leg1.localPosition = (0.25f * transform.right)  + transform.up * AnimMath.Slide(leg1.localPosition.y, 0, 0.1f) + (0.25f* transform.forward);
                leg2.localPosition = (-0.25f * transform.right) + transform.up * AnimMath.Slide(leg2.localPosition.y, 0, 0.1f) + (0.25f * transform.forward);
                leg3.localPosition = (0.25f * transform.right)  + transform.up * AnimMath.Slide(leg3.localPosition.y, 0, 0.1f) + (-0.25f * transform.forward);
                leg4.localPosition = (-0.25f * transform.right) + transform.up * AnimMath.Slide(leg4.localPosition.y, 0, 0.1f) + (-0.25f * transform.forward);
                break;
            case "Walk":
                transform.position = (Mathf.MoveTowards(transform.position.x, player.position.x, Time.deltaTime) * transform.right) + (transform.up * AnimMath.Slide(transform.position.y, 1.25f, 0.1f)) + (Mathf.MoveTowards(transform.position.z, player.position.z, Time.deltaTime) * transform.forward);
                leg1.localPosition = (0.25f * transform.right) - (transform.up * ((Mathf.Sin(Time.time*25)+1)/2)) + (0.25f* transform.forward);
                leg2.localPosition = (-0.25f * transform.right) - (transform.up * ((Mathf.Cos(Time.time*25)+1)/2)) + (0.25f * transform.forward);
                leg3.localPosition = (0.25f * transform.right) - (transform.up * ((Mathf.Sin(Time.time*25 + 1)+1)/2)) + (-0.25f * transform.forward);
                leg4.localPosition = (-0.25f * transform.right) - (transform.up * ((Mathf.Cos(Time.time*25 + 1)+1)/2)) + (-0.25f * transform.forward);
                break;
            case "Fire":
                Vector3 disToTarget = player.position - transform.position;
                rotator.rotation = AnimMath.Slide(rotator.rotation, Quaternion.LookRotation(disToTarget, -Vector3.up));
                break;
            case "Destroyed":
                break;
        }
    }
}
