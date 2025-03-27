using UnityEngine;

public class slopCollect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float rotationSpeed = 55f;
    public float scaleIncrease = 0.2f;
    private const int massIncrease = 2;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            other.transform.localScale += new Vector3(scaleIncrease, scaleIncrease, scaleIncrease);
            if (other.attachedRigidbody.mass < 20)      // TODO: Put this magic 20 in a constants file somewhere
            {
                other.attachedRigidbody.mass += massIncrease;
            }

            Animator animator = other.GetComponent<Animator>();
            animator.SetTrigger("Eat");

            Destroy(gameObject);
        }
    }
}
