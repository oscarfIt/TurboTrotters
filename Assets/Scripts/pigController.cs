using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class pigController : MonoBehaviour
{

    public PigInputActions pigControls;
    public TurboPoints turboPoints;

    [Header("Movement Settings")]

    // Constants
    public float moveSpeed = 5f;
    private float updatedSpeed;
    public float turnSpeed = 200f;
    public float jumpForce = 600f;
    public float groundDistance = 0.3f;
    public float downwardsForce = 200f;

    [Header("Jump Settings")]
    public Transform groundCheck;
    public LayerMask groundMask;

    private Rigidbody rb;
    private Animator animator;
    private Collider playerCollider;
    private bool isGrounded;
    private Vector3 previousPosition;
    private Vector3 originalScale;

    private InputAction move;
    private InputAction jump;
    private InputAction turboBoost;
    private double turboEndTime;
    private bool isBoosted = false;

    [Header("Friction Settings")]
    public float normalDrag = 1f;
    public float iceDrag = 0.2f;
    public float mudDrag = 5f;

    // relates to the ground layer levels
    [Header("Terrain Layer Mapping")]
    public int groundSoilIndex = 1; // Adjust based on your terrain layer order
    public int iceIndex = 2;
    public int mudIndex = 4;

    private float inputHorizontal;
    private float inputVertical;

    // TODO: Probably a better way to deal with these two
    private Vector2 inputDirection = Vector2.zero;
    Vector3 moveDirection = Vector3.zero;

    private void Awake()
    {
        pigControls = new PigInputActions();
        // pigControls.Enable();
    }

    private void OnEnable()
    {
        move = pigControls.Pig.Movement;
        move.Enable();

        jump = pigControls.Pig.Jump;
        jump.Enable();
        jump.performed += Jump;

        turboBoost = pigControls.Pig.TurboBoost;
        turboBoost.Enable();
        turboBoost.performed += TurboBoost;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
    }

    // // Friction values
    // [Header("Friction Materials")]
    // public PhysicsMaterial normalMaterial;
    // public PhysicsMaterial iceMaterial;
    // public PhysicsMaterial mudMaterial;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        previousPosition = transform.position;
        rb.mass = Constants.MIN_MASS;
        turboPoints = new TurboPoints();
        playerCollider = GetComponent<Collider>();
    }

    void Update()
    {
        // Get input
        inputDirection = move.ReadValue<Vector2>();

        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        UpdateFriction(); // Adding Friction based on ground layer

        // Update Animator
        float speed = new Vector2(inputDirection.x, inputDirection.y).magnitude;
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
    }

    void FixedUpdate()
    {
        moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y).normalized;

        // Apply extra downward force if in the air (if it's not moving upwards)
        if (!isGrounded && rb.linearVelocity.y <= 0)
        {
            rb.AddForce(Vector3.down * downwardsForce, ForceMode.Acceleration);
        }

        if (moveDirection.magnitude >= 0.1f)
        {
            // Rotate pig toward movement direction
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);

            // Preserve Y velocity (gravity) while setting X/Z
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 targetVelocity = moveDirection * updatedSpeed;
            rb.linearVelocity = new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z);
        }
        else
        {
            // No movement input � stop horizontal, preserve vertical (gravity)
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        Debug.Log("jumping for joy");
    }

    private void TurboBoost(InputAction.CallbackContext context)
    {
        if (isGrounded && turboPoints.usePoint())
        {
            isBoosted = true;
            animator.SetTrigger("TurboBoost");
            turboEndTime = Time.timeAsDouble + Constants.TURBO_BOOST_DURATION;
            updatedSpeed *= Constants.TURBO_BOOST_MULTIPLIER;
            Debug.Log("Turbo activated!");
        }
    }

    void UpdateFriction()
    {
        int terrainIndex = GetTerrainTextureIndex();

        if (terrainIndex == groundSoilIndex)
        {
            rb.linearDamping = normalDrag;
            updatedSpeed = moveSpeed;
        }
        else if (terrainIndex == iceIndex)
        {
            rb.linearDamping = iceDrag;  // Reduce drag = more slippery
            updatedSpeed = moveSpeed * 1.2f; // Slight speed boost
        }
        else if (terrainIndex == mudIndex)
        {
            rb.linearDamping = mudDrag;  // Increase drag = harder to move
            updatedSpeed = moveSpeed * 0.8f; // Reduce speed
        }
        else
        {
            rb.linearDamping = normalDrag;
            updatedSpeed = moveSpeed;
        }
    }

    int GetTerrainTextureIndex()
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return 0;

        Vector3 playerPos = transform.position;
        Vector3 terrainPos = playerPos - terrain.transform.position;
        TerrainData terrainData = terrain.terrainData;

        float normX = terrainPos.x / terrainData.size.x;
        float normZ = terrainPos.z / terrainData.size.z;

        int mapX = Mathf.Clamp(Mathf.RoundToInt(normX * terrainData.alphamapWidth), 0, terrainData.alphamapWidth - 1);
        int mapZ = Mathf.Clamp(Mathf.RoundToInt(normZ * terrainData.alphamapHeight), 0, terrainData.alphamapHeight - 1);

        float[,,] alphas = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        int maxIndex = 0;
        float maxValue = 0f;

        for (int i = 0; i < alphas.GetLength(2); i++)
        {
            if (alphas[0, 0, i] > maxValue)
            {
                maxIndex = i;
                maxValue = alphas[0, 0, i];
            }
        }

        return maxIndex;
    }

}
