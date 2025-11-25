using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit; // XR 기능 사용 (선택사항)
using UnityEngine.XR.Interaction.Toolkit.Interactables; // 최신 버전용

public class SimpleDoor : MonoBehaviour
{
    [Header("설정")]
    public float openAngle = 0f;   // 열린 상태 각도 (현재 배치 상태)
    public float closeAngle = -90f; // 닫힐 때 각도
    public float speed = 2.0f;

    [Header("옵션")]
    public bool lockAfterClose = true; // ★ 체크하면 한 번 닫히고 끝!

    [Header("회전 축 설정")]
    public Vector3 rotationAxis = new Vector3(0, 0, 1); // Z축

    [Header("사운드")]
    public AudioSource audioSource;
    public AudioClip closeSound; // 닫힐 때 소리

    private Quaternion initialRotation;
    private bool isMoving = false;
    private bool isLocked = false; // 이미 잠겼는지 확인

    void Start()
    {
        initialRotation = transform.localRotation;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    // 외부에서 호출하는 함수
    public void CloseDoorOnce()
    {
        // 이미 움직이는 중이거나, 잠겨있으면 무시
        if (isMoving || isLocked) return;

        // 문 닫기 시작
        StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        isMoving = true;
        
        // 1. 소리 재생
        if (audioSource != null && closeSound != null) audioSource.PlayOneShot(closeSound);

        // 2. 회전 (닫히는 각도로)
        Quaternion startRot = transform.localRotation;
        // 초기 각도(열림)에서 + 닫힘 각도만큼 회전
        Quaternion endRot = initialRotation * Quaternion.Euler(rotationAxis * closeAngle);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
        transform.localRotation = endRot;

        // 3. 잠금 처리 (다시는 못 움직이게)
        isMoving = false;
        if (lockAfterClose)
        {
            isLocked = true;
            
            // (선택) 상호작용 컴포넌트 자체를 꺼버림 -> 손대도 반응 안 함
            var interactable = GetComponent<XRSimpleInteractable>();
            if (interactable != null) interactable.enabled = false;
        }
    }
}