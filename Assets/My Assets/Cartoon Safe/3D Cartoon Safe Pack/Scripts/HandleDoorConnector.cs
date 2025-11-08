using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class HandleDoorConnector : MonoBehaviour
{
    public Rigidbody doorBody;  // Door의 Rigidbody 지정
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    private void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // 잡는 순간 문 Rigidbody를 활성화
        if (doorBody)
            doorBody.isKinematic = false;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // 놓아도 계속 문이 물리로 유지되도록
        if (doorBody)
            doorBody.isKinematic = false;
    }
}
