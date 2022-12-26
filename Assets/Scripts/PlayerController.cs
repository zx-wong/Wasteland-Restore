using UnityEngine;
using static Model;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] CharacterController characterController;

    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector2 viewInput;

    private Vector2 cameraRotation;
    private Vector2 playerRotation;

    private Vector3 newMovementSpeed;
    private Vector3 newMovementVelocity;

    [Header("Camera")]
    [SerializeField] private Transform cameraHolder;

    [Header("Player Settings")]
    public PlayerSettings playerSettings;

    private float minYViewClamp = -70;
    private float maxYViewClamp = 80;

    [Header("Feet")]
    [SerializeField] private Transform feet;
    [SerializeField] LayerMask groundMask;

    public bool isSprint;
    public bool isGround;
    public bool isFall;

    [Header("Gravity")]
    public float gravityAmount;
    public float minGravity;
    public float playerGravity;

    [Header("Jump")]
    public Vector3 jumpForce;
    public Vector3 jumpVelocity;

    [HideInInspector] public float weaponAnimationSpeed;

    private new Rigidbody rigidbody;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip runClip;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Enable();

        playerInput.Player.Movement.performed += i => moveInput = i.ReadValue<Vector2>();
        playerInput.Player.Mouse.performed += i => viewInput = i.ReadValue<Vector2>();
        playerInput.Player.Jump.performed += i => Jump();

        playerInput.Player.Sprint.performed += i => ToggleSprint();
        playerInput.Player.SprintRelease.performed += i => StopSprint();

        cameraRotation = cameraHolder.localRotation.eulerAngles;
        playerRotation = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;

        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        SetGrounded();
        SetFalling();

        CalculateView();
        CalculateMovement();
        CalculateJump();
    }

    private void CalculateView()
    {
        playerRotation.y += playerSettings.senX * (playerSettings.invertX ? -viewInput.x : viewInput.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(playerRotation);

        cameraRotation.x += playerSettings.senY * (playerSettings.invertY ?  viewInput.y : -viewInput.y) * Time.deltaTime;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, minYViewClamp, maxYViewClamp);

        cameraHolder.localRotation = Quaternion.Euler(cameraRotation);
    }    
    
    private void CalculateMovement()
    {
        if (moveInput.y <= .2f)
        {
            isSprint = false;

        }

        var verticalSpeed = playerSettings.walkForward;
        var horizontalSpeed = playerSettings.walkStrafe;

        if (isSprint)
        {
            verticalSpeed = playerSettings.runForward;
            horizontalSpeed = playerSettings.runStrafe;

        }

        if (!isGround)
        {
            playerSettings.speedEffect = playerSettings.fallSpeedEffect;
        }
        else
        {
            playerSettings.speedEffect = 1;
        }

        weaponAnimationSpeed = characterController.velocity.magnitude / (playerSettings.walkForward * playerSettings.speedEffect);

        if (weaponAnimationSpeed > 1)
        {
            weaponAnimationSpeed = 1;
        }

        verticalSpeed *= playerSettings.speedEffect;
        horizontalSpeed *= playerSettings.speedEffect;

        //var movementSpeed = new Vector3(horizontalSpeed * moveInput.x * Time.deltaTime, 0, verticalSpeed* moveInput.y* Time.deltaTime);
        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(horizontalSpeed * moveInput.x * Time.deltaTime, 0, verticalSpeed * moveInput.y * Time.deltaTime), 
            ref newMovementVelocity, (isGround ? playerSettings.movementSmooth : playerSettings.fallSmooth));
        var movementSpeed = transform.TransformDirection(newMovementSpeed);

        //HandleGravity
        if (playerGravity > minGravity)
        {
            playerGravity -= gravityAmount * Time.deltaTime;
        }

        if (playerGravity < -1 && isGround)
        {
            playerGravity = - 1;
        }

        if (jumpForce.y > .1f)
        {
            playerGravity = 0;
        }

        movementSpeed.y += playerGravity;
        movementSpeed += jumpForce * Time.deltaTime;

        source.PlayOneShot(walkClip);
        characterController.Move(movementSpeed);
    }

    private void CalculateJump()
    {
        jumpForce = Vector3.SmoothDamp(jumpForce, Vector3.zero, ref jumpVelocity, playerSettings.jumpFall);
    }

    private void Jump()
    {
        if (!characterController.isGrounded)
        {
            return;
        }

        //Debug.Log("Jump");
        jumpForce = Vector3.up * playerSettings.jumpHeight;
        playerGravity = 0;
    }

    private void ToggleSprint()
    {
        source.PlayOneShot(runClip);

        if (moveInput.y <= .1f)
        {
            isSprint = false;
            return;
        }

        isSprint = !isSprint;
    }

    private void StopSprint()
    {
        if (playerSettings.runHold)
        {
            isSprint = false;
        }
    }

    private void SetGrounded()
    {
        isGround = Physics.CheckSphere(feet.position, playerSettings.groundRadius, groundMask);
    }

    private void SetFalling()
    {
        //Debug.Log(characterController.velocity.magnitude);
        isFall = (!isGround && characterController.velocity.magnitude >= playerSettings.fallSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(feet.position, playerSettings.groundRadius);
    }
}