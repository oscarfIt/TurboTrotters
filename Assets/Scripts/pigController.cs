using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class pigController : MonoBehaviour
{


    [Header("Movement Settings")]
    public float baseSpeed = 5f;
    public float turnSpeed = 200f;
    private float moveSpeed;


    [Header("Jump Settings")]
    public float jumpForce = 6f;
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private Animator animator;
    private Collider playerCollider;
    private bool isGrounded;

    // // Friction values
    // [Header("Friction Materials")]
    // public PhysicsMaterial normalMaterial;
    // public PhysicsMaterial iceMaterial;
    // public PhysicsMaterial mudMaterial;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider>();
    }

    void Update()
    {
        // Get input
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");

        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        UpdateFriction(); // Adding Friction based on ground layer
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Update Animator
        float speed = new Vector2(inputHorizontal, inputVertical).magnitude;
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(inputHorizontal, 0, inputVertical).normalized;

        // Apply extra downward force if in the air
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * 40f, ForceMode.Acceleration);

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
            moveSpeed = baseSpeed;
        }
        else if (terrainIndex == iceIndex)
        {
            rb.linearDamping = iceDrag;  // Reduce drag = more slippery
            moveSpeed = baseSpeed * 1.2f; // Slight speed boost
        }
        else if (terrainIndex == mudIndex)
        {
            rb.linearDamping = mudDrag;  // Increase drag = harder to move
            moveSpeed = baseSpeed * 0.8f; // Reduce speed
        }
        else
        {
            rb.linearDamping = normalDrag;
            moveSpeed = baseSpeed;
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
