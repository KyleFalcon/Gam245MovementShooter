using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class PlayerHealth : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    public TMP_Text HP;
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
        HP.text = "HP: " + currentHealth + "/" + maxHealth;
        if (currentHealth <= 0)
            playerDeath();
    }

    public void takeDamage(float dmg)
    {
        currentHealth -= dmg;
    }
    public void playerDeath()
    {
        //SceneManager.LoadScene(0);
    }
}
