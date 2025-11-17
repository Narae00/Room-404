using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class LeverAttachTrigger : MonoBehaviour
{
    public Rigidbody baseBody;
    public Transform attachPoint;
    public float z_target = 0.1f; 
    private HingeJoint hinge;
    private bool attached = false;

    

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger on ");
        var handle = other.GetComponentInParent<XRGrabInteractable>();
        if (handle == null) return;

        // 이미 부착되어 있으면 무시
        if (attached) return;

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
        hinge.axis = Vector3.right;
        hinge.anchor = new Vector3(0, 0, z_target);
        hinge.useLimits = true;
        var limits = hinge.limits;
        limits.min = -80;
        limits.max = 80;
        hinge.limits = limits;

        hinge.useSpring = false;
        var spring = hinge.spring;
        spring.spring = 120;
        spring.damper = 40;
        hinge.spring = spring;

        attached = true;
        Debug.Log("✅ 레버 핸들 부착됨!");
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
