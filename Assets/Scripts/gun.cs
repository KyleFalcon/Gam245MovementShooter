using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;

    public float knockbackForce = 30f;
    public Rigidbody rb;

    public float bloom;
    public int pellets;

    public int maxAmmo = 10;
    private int currentAmmo;
    public TMP_Text ammoDisplay;
    public RectTransform reticle;
    public float reloadTime = 1f;
    private bool isReloading = false;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public GameObject bulletholePrefab;

    private float nextTimeToFire = 0f;

    public Animator animator;

    void Start()
    {
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
        currentAmmo = maxAmmo;
    }

    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
            return;

        ammoDisplay.text = currentAmmo + " / " + maxAmmo;
        reticle.sizeDelta = new Vector2(bloom, bloom);
        

        if (currentAmmo <= 0) 
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Reload"))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire") && Time.time >=nextTimeToFire)
        {

            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(reloadTime);
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(.25f);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    IEnumerator Recoil()
    {
        animator.SetBool("Firing", true);
        yield return new WaitForSeconds(0.08f);
        animator.SetBool("Firing", false);
    }

    void Shoot()
    {
        StartCoroutine(Recoil());
        muzzleFlash.Play();
        Knockback();
        currentAmmo--;

        Transform t_spawn = fpsCam.transform;

        for (int i = 0; i < Mathf.Max(1, pellets); i++)
        {
            //Bloom
            Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;
            t_bloom += Random.Range(-bloom, bloom) * t_spawn.up;
            t_bloom += Random.Range(-bloom, bloom) * t_spawn.right;
            t_bloom -= t_spawn.position;
            t_bloom.Normalize();

            //Raycast
            RaycastHit hit;
            if (Physics.Raycast(t_spawn.position, t_bloom, out hit, range))
            {
                Debug.Log(hit.transform.name);

                Target target = hit.transform.GetComponent<Target>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                }


                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 1.5f);

                GameObject holeGO = Instantiate(bulletholePrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
                Destroy(holeGO, 3f);
            }
        }
    }

    void Knockback()
    {
        Vector3 behindCamera = -fpsCam.transform.forward;

        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);

        rb.AddForce(behindCamera.normalized * knockbackForce, ForceMode.Impulse);

    }



}


