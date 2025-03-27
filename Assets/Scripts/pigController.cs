using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class pigController : MonoBehaviour
{

    public PlayerInput pigControls;
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
    private double turboEndTime;
    private bool jumped = false;
    private bool boosted = false;   // This is triggered by input
    private bool boosting = false;   // This is for knowing to reset the speed

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        pigControls = GetComponent<PlayerInput>();
        originalScale = transform.localScale;
        previousPosition = transform.position;
        rb.mass = Constants.MIN_MASS;
        turboPoints = new TurboPoints();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumped = context.action.triggered;
    }


    public void OnTurboBoost(InputAction.CallbackContext context)
    {
        boosted = context.action.triggered;
    }


    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        UpdateFriction(); // Adding Friction based on ground layer

        // Update Animator
        float speed = new Vector2(inputDirection.x, inputDirection.y).magnitude;
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
        if (boosted && isGrounded && turboPoints.usePoint())
        {
            boosting = true;
            turboEndTime = Time.timeAsDouble + Constants.TURBO_BOOST_DURATION;
            moveSpeed *= Constants.TURBO_BOOST_MULTIPLIER;
            Debug.Log("Turbo activated!");
        }
        if (jumped && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("Jumped!");
        }
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
            // No movement input, so set X/Z velocity to 0 while preserving Y velocity (gravity) and stop rotation
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            rb.angularVelocity = Vector3.zero;
        }
        float distanceTravelled = Vector3.Distance(previousPosition, transform.position);
        previousPosition = transform.position;
        CardioEffect(distanceTravelled);

        if (boosting && Time.timeAsDouble >= turboEndTime)
        {
            boosting = false;
            moveSpeed /= Constants.TURBO_BOOST_MULTIPLIER;
            Debug.Log("Turbo ended!");
        }
    }

    // FIXME: Scale and mass changes at different rates
    private void CardioEffect(float distanceTravelled)
    {
        float newMass = rb.mass - (Constants.DISTANCE_MASS_DECREASE * distanceTravelled);
        Vector3 newScale = transform.localScale - new Vector3(Constants.DISTANCE_SCALE_DECREASE, Constants.DISTANCE_SCALE_DECREASE, Constants.DISTANCE_SCALE_DECREASE);
        if (newMass > Constants.MIN_MASS)
            // No movement input � stop horizontal, preserve vertical (gravity)
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
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
            animator.SetTrigger("Eat");
        }
        return maxIndex;
    }

}
