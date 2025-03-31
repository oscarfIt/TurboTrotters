using UnityEngine;

public class jumpPad : MonoBehaviour
{
    public float hoverSpeed = 2f;
    public float hoverHeight = 0.2f;  // Max height variation
    public float jumpForce = 20f;  // Force applied to the player
    private Vector3 startPosition;

    private AudioSource audioSource;
    private SphereCollider padCollider;
    private bool hovering = true;
    void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Hover(hovering);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Apply an explosive force upwards
                playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, jumpForce, playerRb.linearVelocity.z);
                audioSource.Play();
            }
        }
    }
    private void Hover(bool isMoving)
    {
        if (isMoving)
        {
            // transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }
    }
}
