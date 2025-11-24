using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class KeyWithHinge : MonoBehaviour
{
    [Header("References")]
    public Transform insertPoint;
    public HingeJoint hinge;
    public Rigidbody keyJointRootRb;
    public Collider keyHoleTrigger;
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    [Header("Unlock Settings")]
    public float unlockAngle = 60f;        // 이 각도 이상 회전하면 언락
    public UnityEvent onUnlocked;

    [Header("Behavior")]
    public bool autoSnap = true;
    public bool disableKeyPullOut = true;

    private bool inserted = false;
    private bool unlocked = false;

    private float startAngle;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hinge.connectedBody = null;
        grab ??= GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == keyHoleTrigger)
            InsertKey();
    }

    private void InsertKey()
    {
        if (inserted) return;
        inserted = true;

        // 스냅 적용
        if (autoSnap)
        {
            rb.transform.SetPositionAndRotation(insertPoint.position, insertPoint.rotation);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // hinge 연결
        hinge.connectedBody = rb;

        if (disableKeyPullOut)
            rb.constraints = RigidbodyConstraints.FreezePosition;

        // 다음 물리 프레임에서 startAngle을 안전하게 설정
        StartCoroutine(InitStartAngleNextPhysicsFrame());
    }

    private System.Collections.IEnumerator InitStartAngleNextPhysicsFrame()
    {
        yield return new WaitForFixedUpdate();  // ⚠ hinge 초기화 기다림

        startAngle = hinge.angle;

        // 안전장치: NaN 방지
        if (float.IsNaN(startAngle))
            startAngle = 0f;

        // 디버그용
        Debug.Log("[KeyWithHinge] Start Angle 설정됨 → " + startAngle);
    }

    void Update()
    {
        if (!inserted || unlocked)
            return;

        float currentAngle = hinge.angle;

        // NaN 보호
        if (float.IsNaN(currentAngle))
            return;


        // 디버그 출력
        Debug.Log($"[Angle]: {currentAngle}");

        // 조건 만족하면 로그 + 이벤트 실행
        if (85f <= Mathf.Abs(currentAngle) && Mathf.Abs(currentAngle) <= 93f)
        {
            unlocked = true;

            Debug.Log("🔓 Unlock 조건 만족! 이벤트 실행됨");
            onUnlocked?.Invoke();

            // 🔥 열쇠 삭제
            Destroy(gameObject);
        }
    }
}
