using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class pigController : MonoBehaviour
{
    public Camera mainCamera;
    public RaceManager raceManager;
    public PlayerInput pigControls;
    public TurboPoints turboPoints;

    [Header("Movement Settings")]

    // Constants
    public float turnSpeed = 200f;
    public float groundDistance = 0.3f;

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
    private bool beingKicked = false;   // This should probably disable some stuff

    [Header("Friction Settings")]

    // relates to the ground layer levels
    [Header("Terrain Layer Mapping")]
    public int groundSoilIndex = 1; // Adjust based on your terrain layer order
    public int iceIndex = 2;
    public int mudIndex = 4;
    private float baseSpeed = 20f;
    private float currentSpeed = 0f;
    private float jumpForce = 600f;
    private float downwardsForce = 300f;

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
        mainCamera = Camera.main;
        if (raceManager == null)
            raceManager = GameObject.FindGameObjectsWithTag("RaceManager")[0].GetComponent<RaceManager>();
        transform.localScale = new Vector3(PigScale.MIN, PigScale.MIN, PigScale.MIN);
        minScaleMagnitude = transform.localScale.magnitude;
        previousPosition = transform.position;
        currentSpeed = baseSpeed;
        rb.mass = PigMass.MIN;
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

        if (isGrounded)
            beingKicked = false;
        UpdateSpeed();
        UpdateMoveAnimations(speed);
        UpdateSounds(speed);
        UpdateJump();
    }

    void FixedUpdate()
    {
        moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y).normalized;

        if (!IsInView() && !beingKicked)
        {
            StartCoroutine(raceManager.KickPig(gameObject));
        }

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
        baseSpeed = Movement.SPEED_NUMERATOR / rb.mass;
        currentSpeed = baseSpeed * terrainMultiplier * turboMultiplier;
    }

    private void UpdateJump()
    {
        if (jumped && isGrounded)
        {
            StartCoroutine(SmoothJump());
            if (jumpSound != null) { audioSource.PlayOneShot(jumpSound, 0.5f); }
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
            turboEndTime = Time.timeAsDouble + TurboBoost.DURATION;
            Debug.Log("Turbo activated!");
        }
        if (boosting)
        {
            if (Time.timeAsDouble >= turboEndTime)  // Boost has ended
            {
                boosting = false;
                multiplier = (1/TurboBoost.SPEED_MULTIPLIER);
                Debug.Log("Turbo ended!");
            }
            else // Boost is active
            {
                multiplier = TurboBoost.SPEED_MULTIPLIER;
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
        if (speed > Movement.DETECTION_THRESHOLD && isGrounded)
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

    private void CardioEffect(float distanceTravelled)
    {
        if (distanceTravelled < Movement.DETECTION_THRESHOLD) return;
        float newMass = rb.mass - (PigMass.DISTANCE_DECREASE * distanceTravelled);
        float newScaleComponent = PigScale.DISTANCE_DECREASE * distanceTravelled;
        Vector3 newScale = transform.localScale - new Vector3(newScaleComponent, newScaleComponent, newScaleComponent);
        rb.mass = Mathf.Clamp(newMass, PigMass.MIN, PigMass.MAX);
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
            EatSlop(1);
            animator.SetTrigger("Eat");
        }
        else if (other.CompareTag("FinishLine"))
        {
            raceManager.NextLap(gameObject.name);
            Debug.Log($"{gameObject.name} CROSSED THE FINISH LINE!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Player {collision.gameObject.name} collided with {gameObject.name}");
            Rigidbody otherPigRb = collision.gameObject.GetComponent<Rigidbody>();
            if (otherPigRb.mass > rb.mass)
            {
                float massDiff = otherPigRb.mass - rb.mass;
                Debug.Log($"Mass difference: {massDiff}");
                // Get pushed away from the other pig
                Vector3 pushDirection = (transform.position - collision.transform.position).normalized;
                StartCoroutine(SmoothCollisionForce(pushDirection, massDiff));
            }
        }
    }

    private float GetFrictionMultiplier()
    {
        int terrainIndex = GetTerrainTextureIndex();
        float multiplier = 1f;

        if (terrainIndex == groundSoilIndex)
        {
            rb.linearDamping = TerrainFriction.DEFAULT_DRAG;  // Normal drag
            multiplier = TerrainFriction.DEFAULT_SPEED_MULTIPLIER; // Normal speed
        }
        else if (terrainIndex == iceIndex)
        {
            rb.linearDamping = TerrainFriction.ICE_DRAG;  // Reduce drag = more slippery
            multiplier = TerrainFriction.ICE_SPEED_MULTIPLIER; // Slight speed boost
        }
        else if (terrainIndex == mudIndex)
        {
            rb.linearDamping = TerrainFriction.MUD_DRAG;  // Increase drag = harder to move
            multiplier = TerrainFriction.MUD_SPEED_MULTIPLIER; // Reduce speed
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

    private void EatSlop(int factor)
    {
        if (rb.mass < PigMass.MAX - factor * PigMass.SLOP_INCREASE)
        {
            rb.mass += factor * PigMass.SLOP_INCREASE;
            transform.localScale += new Vector3(factor * PigScale.SLOP_INCREASE, factor * PigScale.SLOP_INCREASE, factor * PigScale.SLOP_INCREASE);
        }
    }

    // For checking if a pig falls too far behind
    // KICK IT BACK!
    private bool IsInView()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
        return viewportPosition.x > 0f && viewportPosition.z > 0f;
    }

    private System.Collections.IEnumerator SmoothJump()
    {
        float elapsedTime = 0f;

        while (elapsedTime < Movement.JUMP_DURATION)
        {
            float force = Mathf.Lerp(jumpForce, 0, elapsedTime / Movement.JUMP_DURATION);
            rb.AddForce(Vector3.up * force * Time.fixedDeltaTime, ForceMode.Acceleration);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (jumpSound != null) { audioSource.PlayOneShot(jumpSound); }
    }

    private System.Collections.IEnumerator SmoothCollisionForce(Vector3 pushDirection, float massDiff)
    {
        float elapsedTime = 0f;

        while (elapsedTime < Movement.COLLISION_PUSH_DURATION)
        {
            float force = Mathf.Lerp(massDiff * PigMass.COLLISION_FORCE, 0, elapsedTime / Movement.COLLISION_PUSH_DURATION);
            rb.AddForce(pushDirection * force * Time.fixedDeltaTime, ForceMode.Acceleration);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

}
