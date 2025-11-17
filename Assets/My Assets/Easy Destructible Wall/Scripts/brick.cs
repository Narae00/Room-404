using UnityEngine;
using System.Collections;

public class BreakableChunk : MonoBehaviour
{
    public float breakForce = 3.0f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Hammer")) return;

        // 충돌 세기가 충분한지 체크
        if (collision.relativeVelocity.magnitude < breakForce)
            return;

        Break();
    }

    void Break()
    {
        rb.isKinematic = false;     // 물리 작동 시작
        rb.useGravity = true;
    }
}
