using UnityEngine;

public class Target : MonoBehaviour
{

    public float health = 50f;
    Animator animator;

    public void Start()
    {
        animator.GetComponent<Animator>();
    }

    public void TakeDamage (float amount)
    {
        health -= amount;
        if (this.CompareTag("Enemy")) animator.SetTrigger("Damage");
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
