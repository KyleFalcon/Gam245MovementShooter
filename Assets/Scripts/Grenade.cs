using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float damage;
    public float radius = 5f;
    public float force = 2000;
    public ParticleSystem explosion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            PlayerHealth playerHealth = nearbyObject.GetComponent<PlayerHealth>();

            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
            float distance = Vector3.Distance(transform.position, nearbyObject.transform.position);
            float damageMultiplier = 0;
            if(distance <= radius)
            {
                damageMultiplier = (radius - distance) / radius;
            }

            if(playerHealth != null)
            {
                playerHealth.takeDamage(damage * damageMultiplier);
            }
        }

        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
