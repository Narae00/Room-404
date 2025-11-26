using UnityEngine;

public class CoverStayOpen : MonoBehaviour
{
    [Header("설정")]
    public HingeJoint hinge;
    public float openThreshold = 80f; // 이 각도 넘으면 고정됨
    public float holdForce = 10f;     // 고정하는 힘 (스프링 강도)

    void Update()
    {
        if (hinge == null) return;

        // 현재 덮개의 각도
        float currentAngle = hinge.angle;

        // ★ 1. 각도가 80도(설정값)를 넘었으면? -> 고정!
        if (currentAngle > openThreshold)
        {
            JointSpring spring = hinge.spring;
            spring.spring = holdForce;       // 힘을 줌
            spring.targetPosition = hinge.limits.max; // 완전히 열린 각도로 당김
            hinge.spring = spring;
            hinge.useSpring = true;          // 스프링 켜기
        }
        // ★ 2. 각도가 낮으면? -> 해제 (중력으로 닫히게)
        else
        {
            hinge.useSpring = false;         // 스프링 끄기
        }
    }
}