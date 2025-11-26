using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // XR 기능
using UnityEngine.XR.Interaction.Toolkit.Interactables; // 최신 버전용

public class CoverLock : MonoBehaviour
{
    [Header("컴포넌트 연결")]
    public Rigidbody rb;
    public XRGrabInteractable grab;

    [Header("사운드")]
    public AudioSource audioSource;
    public AudioClip unlockSound; // "철컥" 소리

    void Start()
    {
        // 시작할 때: 잠금 상태
        if (rb) rb.isKinematic = true; // 물리 고정 (안 움직임)
        if (grab) grab.enabled = false; // 잡기 불가
    }

    // 키패드 성공 시 호출할 함수
    public void Unlock()
    {
        // 1. 물리 풀기
        if (rb) rb.isKinematic = false;

        // 2. 잡기 허용
        if (grab) grab.enabled = true;

        // 3. 소리 재생
        if (audioSource && unlockSound) audioSource.PlayOneShot(unlockSound);

        Debug.Log("덮개 잠금 해제됨!");
    }
}