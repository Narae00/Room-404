using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KeyAttachTrigger : MonoBehaviour
{
    [Header("Attach Settings")]
    public Transform attachPoint;         // 키가 꽂히는 위치
    public Rigidbody lockBody;            // 금고 몸체 (hinge 연결 대상)
    public Vector3 hingeAxis = Vector3.forward;
    public Vector3 anchorLocal = Vector3.zero;

    private XRGrabInteractable grab;
    private Rigidbody keyRb;
    private HingeJoint hinge;

    private bool inserted = false;

    private Quaternion lastControllerRot;

    private void Awake()
    {
        grab = GetComponentInParent<XRGrabInteractable>();
        keyRb = GetComponentInParent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (inserted) return;

        // key 들어왔는지 확인
        if (!other.transform.IsChildOf(transform.parent)) return;

        InsertKey();
    }

    private void InsertKey()
    {
        inserted = true;

        // 1) 스냅
        keyRb.transform.SetPositionAndRotation(attachPoint.position, attachPoint.rotation);
        keyRb.linearVelocity = Vector3.zero;
        keyRb.angularVelocity = Vector3.zero;

        // 2) ThrowOnDetach 꺼서 kinematic 문제 방지
        grab.throwOnDetach = false;

        // 3) 손에서 강제 놓기 (Detach)
        if (grab.interactorsSelecting.Count > 0)
        {
            var interactor = grab.interactorsSelecting[0];
            grab.interactionManager.SelectExit(interactor, grab);
        }

        // 4) Grab 비활성화 (더 이상 잡히지 않도록)
        grab.enabled = false;

        // 5) 위치는 고정 → 이동으로 인한 torque 제거
        keyRb.constraints = RigidbodyConstraints.FreezePosition;

        // 6) 키에 hinge 붙이기 (회전 제한만 적용)
        hinge = keyRb.gameObject.AddComponent<HingeJoint>();
        hinge.connectedBody = lockBody;

        hinge.axis = hingeAxis;
        hinge.anchor = anchorLocal;

        hinge.useLimits = true;
        JointLimits limits = new JointLimits();
        limits.min = 0;
        limits.max = 90;
        hinge.limits = limits;

        // 7) 컨트롤러 회전 저장
        if (grab.interactorsSelecting.Count > 0)
            lastControllerRot = grab.interactorsSelecting[0].transform.rotation;

        Debug.Log("🔑 키가 삽입됨");
    }

    private void Update()
    {
        if (!inserted) return;

        // grab은 꺼졌지만 interactor 정보는 남아 있음
        if (grab.interactorsSelecting.Count == 0) return;

        var controller = grab.interactorsSelecting[0].transform;

        // 회전 변화량 계산
        Quaternion delta = controller.rotation * Quaternion.Inverse(lastControllerRot);

        // 키에 회전 적용 (transform 기반이라 매우 부드러움)
        keyRb.transform.rotation = delta * keyRb.transform.rotation;

        // hinge 한계가 넘어가면 hinge가 자동으로 clipping 시켜줌
        lastControllerRot = controller.rotation;
    }
}
