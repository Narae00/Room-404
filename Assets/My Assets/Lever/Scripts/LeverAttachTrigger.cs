using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;



public class LeverAttachTrigger : MonoBehaviour
{   
    public int leverIndex;
    public LeverPuzzleController controller;
    public Rigidbody baseBody;
    public Transform attachPoint;
    public Vector3 axis;
    public Vector3 anchor;
    private HingeJoint hinge;
    private bool attached = false;

    private void Update()
    {
        if (!attached || hinge == null) return;

        // 현재 각도 출력
        // Debug.Log("현재 레버 각도 = " + hinge.angle);

        // 각도에 따라서 3 단계로 조절
        if (hinge.angle > 50f)
        {
            SetLeverAngle(70f);
            controller.SetLeverState(leverIndex, true);
        }
        else if (hinge.angle < -50f)
        {
            SetLeverAngle(-70f);
            controller.SetLeverState(leverIndex, false);
        }
        else
        {
            SetLeverAngle(0f);
            controller.SetLeverState(leverIndex, false);
        }
    }

    public void SetLeverAngle(float targetAngle)
    {
        if (!attached || hinge == null) return;

        hinge.useSpring = true; // 반드시 켜야 동작
        var spring = hinge.spring;
        spring.targetPosition = targetAngle;
        hinge.spring = spring;

        // Debug.Log($"레버 목표 각도로 이동 → {targetAngle}");
    }

    private void OnTriggerEnter(Collider other)
    {
        // 이미 부착되어 있으면 무시
        if (attached) return;
        // Debug.Log("Trigger on ");
        var handle = other.GetComponentInParent<XRGrabInteractable>();
        if (handle == null) return;

        // 아직 손에 들려 있는 상태라면(Grab 중) => 부착 X
        if (handle.isSelected) return;

        handle.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        // Rigidbody 가져오기
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        // 강제 위치 정렬
        rb.transform.SetPositionAndRotation(attachPoint.position, attachPoint.rotation);

        // 물리 세팅
        rb.isKinematic = false;
        rb.useGravity = false;

        // HingeJoint 부착
        hinge = rb.gameObject.AddComponent<HingeJoint>();
        hinge.connectedBody = baseBody;
        hinge.axis = axis;
        hinge.anchor = anchor;
        hinge.useLimits = true;
        var limits = hinge.limits;
        limits.min = -80;
        limits.max = 80;
        hinge.limits = limits;

        hinge.useSpring = true;
        var spring = hinge.spring;
        spring.spring = 60;
        spring.damper = 25;
        hinge.spring = spring;

        attached = true;

        Debug.Log($"✅ {leverIndex}번 핸들 부착됨!");
    }

    

    private void OnTriggerExit(Collider other)
    {
        var handle = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (handle == null) return;
        if (!attached) return;

        var rb = other.attachedRigidbody;
        if (rb == null) return;

        // ✅ HingeJoint 제거 (Base에서 분리)
        var joint = rb.GetComponent<HingeJoint>();
        if (joint != null)
        {
            Destroy(joint);
            attached = false;
            Debug.Log("레버 핸들 분리됨!");
        }
    }
}
