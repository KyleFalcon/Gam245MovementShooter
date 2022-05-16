using UnityEngine;

public class Target : MonoBehaviour
{

    public float health = 50f;
    Animator animator;

    public void Start()
    {
    }

    public void TakeDamage (float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();

        }


    }

    void Die()
    {
        Destroy(gameObject);


    }

}
