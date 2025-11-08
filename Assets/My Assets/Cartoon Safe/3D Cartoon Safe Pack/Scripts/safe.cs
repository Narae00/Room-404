using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorHandleTorqueController : MonoBehaviour
{
    [Header("핸들 오브젝트")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable handle;

    [Header("문 회전 힘 설정")]
    public Rigidbody doorBody;
    public float torqueStrength = 10f;

    private bool isGrabbed = false;
    private Transform handleTransform;
    private Vector3 lastPos;

    private void Start()
    {
        if (handle != null)
        {
            handle.selectEntered.AddListener(OnGrab);
            handle.selectExited.AddListener(OnRelease);
            handleTransform = handle.transform;
        }
    }

    private void OnDestroy()
    {
        if (handle != null)
        {
            handle.selectEntered.RemoveListener(OnGrab);
            handle.selectExited.RemoveListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        lastPos = handleTransform.position;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    private void FixedUpdate()
    {
        if (isGrabbed && doorBody != null && handleTransform != null)
        {
            // 핸들의 이동 벡터 계산
            Vector3 delta = handleTransform.position - lastPos;

            // 문에 회전력(Torque) 적용 (문이 Y축 기준으로 열릴 때)
            doorBody.AddTorque(transform.up * delta.x * torqueStrength, ForceMode.Force);

            lastPos = handleTransform.position;
        }
    }
}
