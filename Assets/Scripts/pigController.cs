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
    private Vector3 previousPosition;
    private float minScaleMagnitude;
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
    private float baseSpeed = 5f;
    private float currentSpeed = 0f;

    // TODO: Probably a better way to deal with these two
    private Vector2 inputDirection = Vector2.zero;
    Vector3 moveDirection = Vector3.zero;

    public AudioClip jumpSound;
    private AudioSource audioSource;
    public AudioClip footstepSound;
    public float stepInterval = 0.3f;
    private float stepTimer;

    void Start()
    {
        // HUDManager.Instance.UpdateHUDPosition(1, 2);
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        pigControls = GetComponent<PlayerInput>();
        transform.localScale = new Vector3(Constants.MIN_SCALE, Constants.MIN_SCALE, Constants.MIN_SCALE);
        minScaleMagnitude = transform.localScale.magnitude;
        previousPosition = transform.position;
        currentSpeed = baseSpeed;
        rb.mass = Constants.MIN_MASS;
        turboPoints = new TurboPoints();
        audioSource = GetComponent<AudioSource>();
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
        float speed = new Vector2(inputDirection.x, inputDirection.y).magnitude;

        UpdateSpeed();
        UpdateMoveAnimations(speed);
        UpdateSounds(speed);
        UpdateJump();
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
            Vector3 targetVelocity = moveDirection * currentSpeed;
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
    }

    private void UpdateSpeed()
    {
        // Calculated based on terrain and turbo boost
        float terrainMultiplier = GetFrictionMultiplier();
        float turboMultiplier = GetTurboMultiplier();
        currentSpeed = baseSpeed * terrainMultiplier * turboMultiplier;
    }

    private void UpdateJump()
    {
        if (jumped && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (jumpSound != null) { audioSource.PlayOneShot(jumpSound); }
            jumped = false;
            Debug.Log("Jumped!");
        }
    }
    
    private float GetTurboMultiplier()
    {
        float multiplier = 1f;
        if (boosted && isGrounded && turboPoints.usePoint())
        {
            boosting = true;
            boosted = false;
            turboEndTime = Time.timeAsDouble + Constants.TURBO_BOOST_DURATION;
            Debug.Log("Turbo activated!");
        }
        if (boosting)
        {
            if (Time.timeAsDouble >= turboEndTime)  // Boost has ended
            {
                boosting = false;
                multiplier = (1/Constants.TURBO_BOOST_MULTIPLIER);
                Debug.Log("Turbo ended!");
            }
            else // Boost is active
            {
                multiplier = Constants.TURBO_BOOST_MULTIPLIER;
            }
        }
        return multiplier;
    }

    private void UpdateMoveAnimations(float speed)
    {
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void UpdateSounds(float speed)
    {
        if (speed > Constants.MOVE_DETECTION_THRESHOLD && isGrounded)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                if (footstepSound != null && audioSource != null) { audioSource.PlayOneShot(footstepSound); }
                stepTimer = 0f;
            }
        }
        else { stepTimer = stepInterval; }
    }

    // FIXME: Scale and mass changes at different rates
    private void CardioEffect(float distanceTravelled)
    {
        if (distanceTravelled < Constants.MOVE_DETECTION_THRESHOLD) return;
        float newMass = rb.mass - (Constants.DISTANCE_MASS_DECREASE * distanceTravelled);
        float newScaleComponent = Constants.DISTANCE_SCALE_DECREASE * distanceTravelled;
        Vector3 newScale = transform.localScale - new Vector3(newScaleComponent, newScaleComponent, newScaleComponent);
        rb.mass = Mathf.Clamp(newMass, Constants.MIN_MASS, Constants.MAX_MASS);
        if (newScale.magnitude >= minScaleMagnitude)
        {
            transform.localScale = newScale;        
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TurboPickup"))
        {
            turboPoints.add();
        }
        else if (other.CompareTag("Slop"))
        {
            transform.localScale += new Vector3(Constants.SLOP_SCALE_INCREASE, Constants.SLOP_SCALE_INCREASE, Constants.SLOP_SCALE_INCREASE);
            float newMass = rb.mass + Constants.SLOP_MASS_INCREASE;
            if (newMass < Constants.MAX_MASS)
            {
                rb.mass += Constants.SLOP_MASS_INCREASE;
            }
            else if (rb.mass < Constants.MAX_MASS)
            {
                rb.mass = Constants.MAX_MASS;
            }
            animator.SetTrigger("Eat");
        }
    }

    private float GetFrictionMultiplier()
    {
        int terrainIndex = GetTerrainTextureIndex();
        float multiplier = 1f;

        if (terrainIndex == groundSoilIndex)
        {
            rb.linearDamping = normalDrag;  // Normal drag
            multiplier = 1f; // Normal speed
        }
        else if (terrainIndex == iceIndex)
        {
            rb.linearDamping = iceDrag;  // Reduce drag = more slippery
            multiplier = 1.2f; // Slight speed boost
        }
        else if (terrainIndex == mudIndex)
        {
            rb.linearDamping = mudDrag;  // Increase drag = harder to move
            multiplier = 0.8f; // Reduce speed
        }
        return multiplier;
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
