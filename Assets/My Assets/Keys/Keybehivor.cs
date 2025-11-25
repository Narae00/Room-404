using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class KeyBehavior : MonoBehaviour
{
    public Transform keyLock;
    public UnityEvent SafeEvent;
    private Quaternion initialRotation;
    private bool isInserted = false;
    private bool isGrabbed = false;
    private bool isUnlock = false;
    private Quaternion keyLockInitialRot;
    private float angle;

    void Awake()
    {
        var grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrabbed);
        grab.selectExited.AddListener(_ => isGrabbed = false);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        if (isInserted)
        {
            // 그랩 순간의 회전을 초기 기준으로 설정
            initialRotation = transform.localRotation;
            isGrabbed = true;
        }
    }

    void Update()
    {   
        if (isUnlock) return ;

        if (isInserted && isGrabbed)
        {
            float signedAngle = GetSignedAngle(initialRotation, transform.localRotation, Vector3.up);

            Debug.Log($"🔑 회전각(부호 포함): {signedAngle:F2}");

            if (keyLock != null)
            {
                // 키를 돌린 만큼 키홀도 회전시키기 (Y축 기준)
                keyLock.localRotation = keyLockInitialRot * Quaternion.Euler(0, 0, -signedAngle);
            }

            if (signedAngle > 90f)
            {
                isUnlock = true;
                
                SafeEvent?.Invoke();

                Destroy(gameObject);
            }
        }
    }

    public void InsertIntoKeyhole(Transform point)
    {
        isInserted = true;

        transform.position = point.position;
        transform.rotation = point.rotation;

        var rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePosition |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;

        angle = Quaternion.Angle(initialRotation, transform.localRotation);

        // 🔥 키홀의 초기 회전 저장
        if (keyLock != null)
            keyLockInitialRot = keyLock.localRotation;
    }

    float GetSignedAngle(Quaternion from, Quaternion to, Vector3 axis)
    {
        // 두 쿼터니언을 비교하여 상대 회전값 계산
        Quaternion delta = Quaternion.Inverse(from) * to;

        // 쿼터니언을 각도/축 형태로 변환
        delta.ToAngleAxis(out float angle, out Vector3 angleAxis);

        // 각도는 0~180 양수이므로, 축 방향을 보고 부호를 붙여줌
        float sign = Mathf.Sign(Vector3.Dot(angleAxis, axis));

        return angle * sign;
    }
}
