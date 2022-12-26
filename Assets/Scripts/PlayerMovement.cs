using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform orientation;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;

    [SerializeField] private float groundDrag;
    [SerializeField] public bool moving;
    [SerializeField] bool running;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] bool canJump;

    [Header("On Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] bool grounded;

    private float horizontalInput, verticalInput;
    private Vector3 moveDirection;

    [SerializeField] new Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
    }

    private void Update()
    {
        HandleInput();
        HandleSpeed();

        HandleGroundCheck();
        HandleDrag();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput= Input.GetAxisRaw("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        if (Input.GetKey(KeyCode.Space) && canJump && grounded)
        {
            //Debug.Log("Jump");
            canJump = false;
            HandleJump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && grounded)
        {
            running = true;
        }
    }

    private void HandleMovement()
    {
        moveDirection = (orientation.forward * verticalInput) + (orientation.right * horizontalInput);

        if (grounded)
        {
            if (running)
            {
                rigidbody.AddForce(moveDirection.normalized * runSpeed * 10f, ForceMode.Force);
            }
            else
            {
                rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }
        }
        else
        {
            rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void HandleJump()
    {
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
    }

    private void HandleGroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.2f, groundMask);
        //Debug.DrawRay(transform.position, Vector3.down * playerHeight * 0.3f, Color.green);
    }

    private void HandleDrag()
    {
        if (grounded)
        {
            rigidbody.drag = groundDrag;
        }
        else
        {
            rigidbody.drag = 0;
        }
    }

    private void HandleSpeed()
    {
        Vector3 flatVelocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        if (running)
        {
            if (flatVelocity.magnitude > runSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * runSpeed;
                rigidbody.velocity = new Vector3(limitedVelocity.x, rigidbody.velocity.y, limitedVelocity.z);
            }
        }
        else
        {
            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
                rigidbody.velocity = new Vector3(limitedVelocity.x, rigidbody.velocity.y, limitedVelocity.z);
            }
        }
    }
}
