using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAdvanced : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float runSpeed;

    public float groundDrag;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private float slideSpeed;


    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    public float dashForce;
    public float dashCooldown;
    bool readyToDash;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask ground;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    public bool exitingSlope;
    
    public bool slopeCheck;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        running,
        air,
        sliding
    }
    void Start()
    {
        readyToDash = true;
        readyToJump = true;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        //grounded = Physics.CheckSphere(groundCheck.position, groundDistance, ground);
        //Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.2f), Color.blue);
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, ground);

        MyInput();
        SpeedControl();
        if (grounded)
        {
            rb.drag = groundDrag;
            //ResetJump();
        }
        else
        {
            rb.drag = 0;
            //readyToJump = false;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(dashKey) && readyToDash)
        {
            readyToDash = false;

            Dash();
            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    private void StateHandler()
    {
        if (grounded)
        {
            state = MovementState.running;
            moveSpeed = runSpeed;
        }

        else
        {
            state = MovementState.air;
        }
    }
    private void MovePlayer()
    {
        StateHandler();
        //Movement Direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        //Slope Handling 
        slopeCheck = OnSlope();
        if (!exitingSlope && OnSlope()) {
            Debug.DrawRay(transform.position, Vector3.down * slopeHit.distance, Color.green);

            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 120f, ForceMode.Force);
            }

        }
        Debug.DrawRay(transform.position, Vector3.down * slopeHit.distance, Color.red);

        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (OnSlope())
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;


        // rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private void Dash()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        //rb.velocity = Vector3.zero;

        rb.AddForce(inputDirection.normalized * dashForce, ForceMode.Impulse);
    }
    private void ResetDash()
    {
        readyToDash = true;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, (playerHeight * 0.5f) + 0.4f))
        {  
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
