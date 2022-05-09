using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth; 
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
            playerDeath();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Melee"))
        {
            currentHealth -= 10;
        }
    }
    public void playerDeath()
    {
        SceneManager.LoadScene(0);
    }
}
