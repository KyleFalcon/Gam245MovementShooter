using UnityEngine;

public class Target : MonoBehaviour
{

    public float health = 50f;
    Animator animator;
    public bool isEnemy;
    bool isAlive = true;
    GameManager gameManager;

    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void TakeDamage (float amount)
    {
        health -= amount;
        if (health <= 0f && isAlive)
        {
            Die();
        }


    }

    void Die()
    {
        isAlive = false;
        if (isEnemy)
        {
            gameManager.EnemyKilled();
        }
        Destroy(gameObject);
    }

}
