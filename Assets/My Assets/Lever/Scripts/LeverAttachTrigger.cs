using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // 최신 XR Toolkit 기준

public class LeverAttachTrigger : MonoBehaviour
{
    public int leverIndex;
    public LeverPuzzleController controller;
    public Rigidbody baseBody;
    public Transform attachPoint;
    public Vector3 axis;
    public Vector3 anchor;

    [Header("🔊 사운드 설정 (추가됨)")]
    public AudioSource audioSource; // 소리를 낼 스피커 컴포넌트
    public AudioClip attachSound;   // 끼울 때 소리 (철컥)
    public AudioClip pullSound;     // 내릴 때 소리 (딸각 or 웅~)

    private HingeJoint hinge;
    private bool attached = false;
    
    // 소리가 중복해서 계속 나는 것을 방지하기 위한 체크 변수
    private bool hasPlayedPullSound = false; 

    private void Update()
    {
        if (!attached || hinge == null) return;

        // 각도에 따라서 3 단계로 조절
        if (hinge.angle > 50f) // 레버를 내림 (ON)
        {
            SetLeverAngle(70f);
            controller.SetLeverState(leverIndex, true);

            // ★ 내리는 소리 재생 (한 번만 실행되게 체크)
            if (!hasPlayedPullSound)
            {
                PlaySound(pullSound);
                hasPlayedPullSound = true; // 소리 났음! 체크
            }
        }
        else if (hinge.angle < -50f) // 레버를 올림 (OFF)
        {
            SetLeverAngle(-70f);
            controller.SetLeverState(leverIndex, false);
            hasPlayedPullSound = false; // 다시 내릴 때 소리 나게 리셋
        }
        else // 중간 (OFF)
        {
            SetLeverAngle(0f);
            controller.SetLeverState(leverIndex, false);
            hasPlayedPullSound = false; // 리셋
        }
    }

    public void SetLeverAngle(float targetAngle)
    {
        if (!attached || hinge == null) return;

        hinge.useSpring = true; 
        var spring = hinge.spring;
        spring.targetPosition = targetAngle;
        hinge.spring = spring;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (attached) return;

        // XR Toolkit 버전에 따라 경로가 다를 수 있으니 Parent까지 찾음
        var handle = other.GetComponentInParent<XRGrabInteractable>();
        if (handle == null) return;

        // 손에 들고 있으면 부착 안 함
        if (handle.isSelected) return;

        handle.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        // 위치 정렬 및 물리 설정
        rb.transform.SetPositionAndRotation(attachPoint.position, attachPoint.rotation);
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

        // ★ 부착 소리 재생!
        PlaySound(attachSound);

        Debug.Log($"✅ {leverIndex}번 핸들 부착됨!");
    }

    private void OnTriggerExit(Collider other)
    {
        var handle = other.GetComponentInParent<XRGrabInteractable>();
        if (handle == null) return;
        if (!attached) return;

        var rb = other.attachedRigidbody;
        if (rb == null) return;

        var joint = rb.GetComponent<HingeJoint>();
        if (joint != null)
        {
            Destroy(joint);
            attached = false;
            hasPlayedPullSound = false; // 분리되면 소리 체크도 리셋
            Debug.Log("레버 핸들 분리됨!");
        }
    }

    // 소리 재생을 위한 간단한 함수
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}