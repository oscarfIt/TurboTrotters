using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class pigController : MonoBehaviour
{

    public PigInputActions pigControls;
    [Header("Movement Settings")]

    // Constants
    public float moveSpeed = 5f;
    public float turnSpeed = 200f;
    public float jumpForce = 600f;
    public float groundDistance = 0.3f;
    public float downwardsForce = 200f;

    [Header("Jump Settings")]
    public Transform groundCheck;
    public LayerMask groundMask;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;

    private InputAction move;
    private InputAction jump;
    private Vector2 inputDirection = Vector2.zero;

    private void Awake()
    {
        pigControls = new PigInputActions();
    }

    private void OnEnable()
    {
        move = pigControls.Pig.Movement;
        move.Enable();

        jump = pigControls.Pig.Jump;
        jump.Enable();
        jump.performed += Jump;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.mass = Constants.MIN_MASS;
    }

    void Update()
    {
        // Get input
        inputDirection = move.ReadValue<Vector2>();

        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Update Animator
        float speed = new Vector2(inputDirection.x, inputDirection.y).magnitude;
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y).normalized;

        // Apply extra downward force if in the air (if it's not moving upwards)
        if (!isGrounded && rb.linearVelocity.y <= 0)
        {
            Debug.Log("Applying downwards force to pig!");
            rb.AddForce(Vector3.down * downwardsForce, ForceMode.Acceleration);
        }

        if (moveDirection.magnitude >= 0.1f)
        {
            // Rotate pig toward movement direction
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);

            // Preserve Y velocity (gravity) while setting X/Z
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 targetVelocity = moveDirection * moveSpeed;
            rb.linearVelocity = new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z);
        }
        else
        {
            // No movement input, so set X/Z velocity to 0 while preserving Y velocity (gravity) and stop rotation
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("Jumping!");
        }
    }
}
