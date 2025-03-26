using UnityEngine;

public class slopCollect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float rotationSpeed = 55f;
    public float scaleIncrease = 0.2f;


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

            Animator animator = other.GetComponent<Animator>();
            animator.SetTrigger("Eat");

            Destroy(gameObject);
        }
    }
}
