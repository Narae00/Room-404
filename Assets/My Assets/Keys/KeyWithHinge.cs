using UnityEngine;

using UnityEngine.Events;

public class KeyWithHinge : MonoBehaviour
{
    [Header("References")]
    public Transform insertPoint;          // 키가 꽂힐 위치
    public HingeJoint hinge;          // KeyJointRoot에 붙는 Joint
    public Rigidbody keyJointRootRb;       // 축 역할
    public Collider keyHoleTrigger;        // Keyhole Trigger
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    [Header("Unlock Settings")]
    public float unlockAngle = 60f;        // 이 각도 이상 돌리면 Unlock
    public UnityEvent onUnlocked;

    [Header("Behavior")]
    public bool autoSnap = true;           // 들어오면 스냅
    public bool disableKeyPullOut = true;  // 꽂힌 뒤 당겨도 빠지지 않게 함

    private bool inserted = false;
    private bool unlocked = false;

    private float startAngle = 0f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hinge.connectedBody = null;
        grab ??= GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == keyHoleTrigger)
        {
            InsertKey();
        }
    }

    private void InsertKey()
    {
        if (inserted) return;
        inserted = true;

        // 위치·회전 스냅
        if (autoSnap)
        {
            rb.transform.SetPositionAndRotation(insertPoint.position, insertPoint.rotation);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 축에 연결
        hinge.connectedBody = rb;

        startAngle = hinge.angle;

        if (disableKeyPullOut)
        {
            // 키가 더 이상 위치 변하지 않게
            rb.constraints = RigidbodyConstraints.FreezePosition;
        }
    }

    private void Update()
    {
        if (!inserted || unlocked) return;

        float delta = Mathf.Abs(Mathf.DeltaAngle(startAngle, hinge.angle));

        if (delta >= unlockAngle)
        {
            unlocked = true;
            onUnlocked?.Invoke();
        }
    }
}
