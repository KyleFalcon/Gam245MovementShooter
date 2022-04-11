using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 20f;
    public float gravity = -9.81f;
    public float jumpHeight = 10f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;

    Vector3 velocity;


    public LayerMask groundMask;

    bool isGrounded;
    public float jumpsLeft;
    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //Jump and Land
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpsLeft = 2;
        }

        if (Input.GetButtonDown("Jump") && jumpsLeft > 0)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpsLeft--;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Grounded Movement
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
