using UnityEngine;

public class MovingWall : MonoBehaviour
{
    public GameObject wall;
    public Vector3 direction = Vector3.forward;
    public float moveSpeed = 3f;

    private Rigidbody rb;
    private bool moving = false;

    void Start()
    {
        rb = wall.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (moving)
        {
            Vector3 movePos = rb.position + direction.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(movePos);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Player"))
        {
            moving = true;
        }
    }
}
