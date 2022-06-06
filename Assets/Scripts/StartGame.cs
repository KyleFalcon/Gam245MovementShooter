using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{

    public Transform door;
    bool gameStarted;
    // Start is called before the first frame update
    void Start()
    {
        gameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameStarted)
        {
            if(door.position.y < 6.5f)
                closeDoor();
        }
    }

    public void closeDoor()
    {
        door.Translate(-0.1f, 0 , 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !gameStarted)
            gameStarted = true;
    }
}
