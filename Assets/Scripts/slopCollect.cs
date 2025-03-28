using UnityEngine;

public class slopCollect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float rotationSpeed = 55f;

    private AudioSource audioSource;
    private SphereCollider slopCollider;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        slopCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Animator animator = other.GetComponent<Animator>();
            animator.SetTrigger("Eat");
            audioSource.Play();
            slopCollider.enabled = false;


            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
            Destroy(gameObject, audioSource.clip.length);
        }
    }
}
