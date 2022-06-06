using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

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
        currentHealth -= (int)dmg;
    }
    public void playerDeath()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(2);
    }
}
