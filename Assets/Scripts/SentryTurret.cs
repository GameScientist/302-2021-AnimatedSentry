using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryTurret : MonoBehaviour
{
    public AudioManager audioManager;
    public HealthSystem health;
    public string state;
    public Transform rotator;
    public Transform leg1;
    public Transform leg2;
    public Transform leg3;
    public Transform leg4;
    public Transform end;

    public Rigidbody basePrefab;
    public Rigidbody rotatorPrefab;
    public Rigidbody barrelPrefab;
    public Rigidbody legPrefab;

    public Transform player;

    public ParticleSystem flash;
    public ParticleSystem explosion;

    private float cooldown = 0;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<HealthSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown < 1) cooldown += Time.deltaTime;
        else cooldown = 1;

        if (health.dead) state = "Destroyed";
        print(state);
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
                transform.position = (Mathf.MoveTowards(transform.position.x, player.position.x, Time.deltaTime * 5) * transform.right) + (transform.up * AnimMath.Slide(transform.position.y, 1.25f, 0.1f)) + (Mathf.MoveTowards(transform.position.z, player.position.z, Time.deltaTime * 5) * transform.forward);
                leg1.localPosition = (0.25f * transform.right) - (transform.up * ((Mathf.Sin(Time.time*25)+1)/2)) + (0.25f* transform.forward);
                leg2.localPosition = (-0.25f * transform.right) - (transform.up * ((Mathf.Cos(Time.time*25)+1)/2)) + (0.25f * transform.forward);
                leg3.localPosition = (0.25f * transform.right) - (transform.up * ((Mathf.Sin(Time.time*25 + 1)+1)/2)) + (-0.25f * transform.forward);
                leg4.localPosition = (-0.25f * transform.right) - (transform.up * ((Mathf.Cos(Time.time*25 + 1)+1)/2)) + (-0.25f * transform.forward);
                rotator.rotation = AnimMath.Slide(rotator.rotation, Quaternion.LookRotation(player.position - transform.position, -Vector3.up));
                break;

            case "Fire":
                rotator.rotation = AnimMath.Slide(rotator.rotation, Quaternion.LookRotation(player.position - transform.position, -Vector3.up));

                if (cooldown == 1)
                {
                    Instantiate(flash, end.position, end.rotation);
                    HealthSystem playerHealth = player.GetComponent<HealthSystem>();
                    if (playerHealth)
                    {
                        playerHealth.TakeDamage();
                        audioManager.Play("Gun Fire");
                        audioManager.Play("Blood Spurt");
                        audioManager.Play("Player Damage");
                    }
                    cooldown = 0;
                }
                
                break;
            case "Destroyed":
                Rigidbody baseGib;
                baseGib = Instantiate(basePrefab, transform);
                baseGib.transform.SetParent(null);
                baseGib.velocity = transform.TransformDirection((transform.up * Random.Range(-1f, 1f)) + (transform.right * Random.Range(-1f, 1f)) + (transform.forward * Random.Range(-1f, 1f)));
                Rigidbody rotatorGib;
                rotatorGib = Instantiate(rotatorPrefab, transform);
                rotatorGib.transform.SetParent(null);
                rotatorGib.velocity = transform.TransformDirection((transform.up * Random.Range(-1f, 1f)) + (transform.right * Random.Range(-1f, 1f)) + (transform.forward * Random.Range(-1f, 1f)));
                Rigidbody barrelGib;
                barrelGib = Instantiate(barrelPrefab, transform);
                barrelGib.transform.SetParent(null);
                barrelGib.velocity = transform.TransformDirection((transform.up * Random.Range(-1f, 1f)) + (transform.right * Random.Range(-1f, 1f)) + (transform.forward * Random.Range(-1f, 1f)));
                Rigidbody leg1Gib;
                leg1Gib = Instantiate(legPrefab, transform);
                leg1Gib.transform.SetParent(null);
                leg1Gib.velocity = transform.TransformDirection((transform.up * Random.Range(-1f, 1f)) + (transform.right * Random.Range(-1f, 1f)) + (transform.forward * Random.Range(-1f, 1f)));
                Rigidbody leg2Gib;
                leg2Gib = Instantiate(legPrefab, transform);
                leg2Gib.transform.SetParent(null);
                leg2Gib.velocity = transform.TransformDirection((transform.up * Random.Range(-1f, 1f)) + (transform.right * Random.Range(-1f, 1f)) + (transform.forward * Random.Range(-1f, 1f)));
                Rigidbody leg3Gib;
                leg3Gib = Instantiate(legPrefab, transform);
                leg3Gib.transform.SetParent(null);
                leg3Gib.velocity = transform.TransformDirection((transform.up * Random.Range(-1f, 1f)) + (transform.right * Random.Range(-1f, 1f)) + (transform.forward * Random.Range(-1f, 1f)));
                Rigidbody leg4Gib;
                leg4Gib = Instantiate(legPrefab, transform);
                leg4Gib.transform.SetParent(null);
                leg4Gib.velocity = transform.TransformDirection((transform.up * Random.Range(-1f, 1f)) + (transform.right * Random.Range(-1f, 1f)) + (transform.forward * Random.Range(-1f, 1f)));
                ParticleSystem explosionGib;
                explosionGib = Instantiate(explosion, transform.position, Quaternion.Euler(-90, 0, 0));
                explosionGib.transform.SetParent(null);
                audioManager.Play("Explosion");
                audioManager.Play("Electric Sparks 1");
                Destroy(gameObject);
                break;
        }
    }
}
