using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyList;
    public int enemyAmount;
    // Start is called before the first frame update
    void Start()
    {
        enemyAmount = enemyList.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyAmount < 1)
        {
            Win();
        }
    }

    public void Win()
    {
        SceneManager.LoadScene(3);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnemyKilled()
    {
        enemyAmount--;
    }
}
