using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class FinalButton : MonoBehaviour
{
    [Header("1. 버튼 소리 설정")]
    public AudioSource audioSource;
    public AudioClip buttonSound; // 버튼 누르는 "딸각" 소리

    [Header("2. 빛/시각 설정")]
    public GameObject lightObject;
    public Renderer buttonRenderer;
    [ColorUsage(true, true)]
    public Color emissionColor = Color.red * 3f;

    // ▼▼▼ [추가된 부분: 물체 이동 설정] ▼▼▼
    [Header("3. 물체 이동 설정 (2개)")]
    public Transform object1; // 움직일 물체 1 (예: 책상)
    public Transform object2; // 움직일 물체 2 (예: 의자, 또는 반대쪽 문)

    public Vector3 moveAxis = new Vector3(0, 0, 1); // 이동 방향 (X, Y, Z 중 하나를 1로)
    public float moveDistance = 2.0f; // 이동 거리 (미터)
    public float moveSpeed = 1.0f;    // 이동 속도

    public AudioClip moveSound;       // 물체 움직이는 소리 (드르륵, 웅~)
    public float moveDelay = 0.5f;    // 버튼 누르고 몇 초 뒤에 움직일지
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    [Header("4. 마무리 이벤트")]
    public float finishDelay = 2.0f; // 이동 다 끝나고 실행할 이벤트 대기 시간
    public UnityEvent onSequenceFinish;

    private bool isPressed = false;

    public void PressButton()
    {
        if (isPressed) return;
        isPressed = true;

        StartCoroutine(ActionRoutine());
    }

    IEnumerator ActionRoutine()
    {
        // 1. 버튼 반응 (소리 & 빛)
        if (audioSource != null && buttonSound != null)
            audioSource.PlayOneShot(buttonSound);

        if (lightObject != null) lightObject.SetActive(true);

        if (buttonRenderer != null)
            buttonRenderer.material.SetColor("_EmissionColor", emissionColor);

        Debug.Log("🚨 버튼 작동!");

        // 2. 이동 시작 전 딜레이 (잠시 대기)
        yield return new WaitForSeconds(moveDelay);

        // 3. 물체 이동 소리 재생
        if (audioSource != null && moveSound != null)
            audioSource.PlayOneShot(moveSound);

        // 4. 물체 이동 시작 (병렬 실행)
        // 두 물체를 동시에 움직이게 함
        StartCoroutine(MoveObjectRoutine(object1));
        StartCoroutine(MoveObjectRoutine(object2));

        // 5. 마무리 이벤트 대기
        yield return new WaitForSeconds(finishDelay);

        Debug.Log("🎉 모든 시퀀스 종료. 다음 단계 실행.");
        onSequenceFinish?.Invoke();
    }

    // 실제로 물체를 부드럽게 옮기는 함수
    IEnumerator MoveObjectRoutine(Transform target)
    {
        if (target == null) yield break;

        Vector3 startPos = target.position;
        // 목표 위치 = 시작위치 + (방향 * 거리)
        Vector3 endPos = startPos + (moveAxis.normalized * moveDistance);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            // Lerp를 써서 부드럽게 이동
            target.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        // 끝에 도달하면 확실하게 고정
        target.position = endPos;
    }
}