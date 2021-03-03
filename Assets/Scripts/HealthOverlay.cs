using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthOverlay : MonoBehaviour
{
    public HealthSystem health;
    private RawImage image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        image.color = Color.white * (3 - health.health) / 3;
    }
}
