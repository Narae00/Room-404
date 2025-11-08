using UnityEngine;

public class LeverLightController : MonoBehaviour
{
    [Header("연결된 힌지")]
    public HingeJoint hinge;

    [Header("제어할 라이트")]
    public Light targetLight;

    [Header("켜지는 기준 각도 (이 각도 이상이면 켜짐)")]
    public float thresholdAngle = 60f;

    [Header("각도 확인 (디버그용)")]
    public bool showDebug = true;

    private bool lightOn = false;

    private Rigidbody rb;

    void Start()
    {
        if (hinge != null)
        {
            rb = hinge.GetComponent<Rigidbody>();    
        }
        
    }

    void Update()
    {
        if (hinge == null || targetLight == null)
            return;

        float currentAngle = hinge.angle;

        if (showDebug)
            Debug.Log($"[LeverLightController] 현재 각도: {currentAngle:F1} / 라이트 상태: {(lightOn ? "ON" : "OFF")}");

        // 각도 60 이상이면 켜지고, 그 미만이면 꺼짐
        if (!lightOn && currentAngle >= thresholdAngle)
        {
            targetLight.enabled = true;
            lightOn = true;
            rb.isKinematic = true; // 물리 고정

            if (showDebug)
                Debug.Log($"[LeverLightController] 💡 라이트 켜짐 (angle={currentAngle:F1})");
        }
        else if (lightOn && currentAngle < thresholdAngle)
        {
            targetLight.enabled = false;
            lightOn = false;
            rb.isKinematic = false; // 다시 물리 작동

            if (showDebug)
                Debug.Log($"[LeverLightController] 💡 라이트 꺼짐 (angle={currentAngle:F1})");
        }
    }
}
