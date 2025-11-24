using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class XRKeyKnob : MonoBehaviour
{
    [Header("Insert Settings")]
    public Transform insertPoint;
    public Collider keyholeTrigger;

    [Header("Rotation Settings")]
    public float minAngle = -10f;
    public float maxAngle = 90f;
    public float twistSensitivity = 1.5f;

    [Header("Unlock Event")]
    public float unlockAngle = 70f;
    public UnityEvent onUnlocked;

    private bool isInserted = true;
    private bool isGrabbed = false;
    private bool unlocked = false;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private Rigidbody rb;
    private Transform interactor;

    // Rotation Tracking
    private float baseAngle;
    private float currentOffset;
    private float accumulatedAngle;

    private void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == keyholeTrigger && !isInserted)
        {
            Debug.Log("key 접촉 로그");
            InsertKey();
        }
    }

    private void InsertKey()
    {
        isInserted = true;

        // 스냅 고정
        rb.transform.SetPositionAndRotation(insertPoint.position, insertPoint.rotation);
        rb.constraints = RigidbodyConstraints.FreezePosition;  // 위치 고정
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!isInserted) return;

        isGrabbed = true;
        interactor = args.interactorObject.transform;

        ResetRotationTracking();
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        interactor = null;
    }

    private void ResetRotationTracking()
    {
        accumulatedAngle = 0f;
        currentOffset = 0f;
        baseAngle = 0f;
    }

    private void Update()
    {
        if (!isGrabbed || !isInserted || unlocked) return;

        UpdateRotation();
    }

    private void UpdateRotation()
    {
        // Get controller forward vector projected to XZ
        Vector3 localForward = transform.InverseTransformDirection(interactor.forward);
        localForward.y = 0f;
        localForward.Normalize();

        float targetAngle = Mathf.Atan2(localForward.z, localForward.x) * Mathf.Rad2Deg;

        // Find shortest signed angle delta
        float angleDelta = Mathf.DeltaAngle(baseAngle, targetAngle);

        // Smooth tracking
        if (Mathf.Abs(angleDelta) > 90f)
        {
            // 넘어가는 구간이면 누적 후 베이스 변경
            accumulatedAngle += angleDelta;
            baseAngle = targetAngle;
            angleDelta = 0f;
        }

        currentOffset = angleDelta;

        // 총 회전
        float totalAngle = (accumulatedAngle + currentOffset) * twistSensitivity;

        // Clamp
        totalAngle = Mathf.Clamp(totalAngle, minAngle, maxAngle);

        // Apply rotation
        transform.localEulerAngles = new Vector3(0, totalAngle, 0);

        // Unlock condition
        if (totalAngle >= unlockAngle)
        {
            unlocked = true;
            onUnlocked?.Invoke();
        }
    }
}
