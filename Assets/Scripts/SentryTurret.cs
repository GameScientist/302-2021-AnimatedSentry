using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryTurret : MonoBehaviour
{
    private string state;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case "Idle":
                break;
            case "Aim":
                break;
            case "Fire":
                break;
            case "Destroyed":
                break;
        }
    }
}
