using UnityEngine;

public class BathroomDoorController : MonoBehaviour
{
    public HingeJoint hinge; 
    public float openAngle = 140f;        
    public float springForce = 100f;     
    public float damper = 10f;           

    public void OpenDoorByPuzzle()
    {
        if (hinge == null)
        {
            Debug.LogError("HingeJoint reference missing!");
            return;
        }
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        // 스프링 설정
        JointSpring spring = hinge.spring;
        spring.spring = springForce;
        spring.damper = damper;
        spring.targetPosition = openAngle;
        hinge.spring = spring;

        hinge.useSpring = true;

        Debug.Log("🚪 Bathroom Door opened by puzzle!");
    }
}
