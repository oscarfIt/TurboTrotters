using UnityEngine;

public class turboCollect : MonoBehaviour
{
    public float rotationSpeed = 55f;

    private AudioSource audioSource;
    private MeshCollider turboCollider;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        turboCollider = GetComponent<MeshCollider>();
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
            audioSource.Play();
            turboCollider.enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject, audioSource.clip.length);
        }
    }
}
