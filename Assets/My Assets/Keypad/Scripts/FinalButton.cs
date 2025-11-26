using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class FinalButton : MonoBehaviour
{
    [Header("1. 소리 설정")]
    public AudioSource audioSource;
    public AudioClip buttonSound; // 버튼 누르는 소리 (철컥/쾅)

    [Header("2. 빛/시각 설정")]
    public GameObject lightObject; // 켜질 조명 (Spot/Point Light)
    public Renderer buttonRenderer; // 버튼 모델 (색 바꿀 때)
    [ColorUsage(true, true)]
    public Color emissionColor = Color.red * 3f; // 빛나는 색

    [Header("3. 딜레이 및 후속 이벤트")]
    public float delayTime = 1.0f; // 소리 나고 얼마나 기다릴지
    public UnityEvent onSequenceFinish; // 딜레이 후 실행할 것 (탈출구 열기 등)

    private bool isPressed = false;

    // ★ 이 함수를 XR 이벤트에 연결하세요
    public void PressButton()
    {
        if (isPressed) return; // 한 번만 눌리게
        isPressed = true;

        StartCoroutine(ActionRoutine());
    }

    IEnumerator ActionRoutine()
    {
        // 1. 소리 재생
        if (audioSource != null && buttonSound != null)
            audioSource.PlayOneShot(buttonSound);

        // 2. 조명 켜기 (오브젝트 활성화)
        if (lightObject != null)
            lightObject.SetActive(true);

        // 3. 버튼 자체 발광 (Emission)
        if (buttonRenderer != null)
            buttonRenderer.material.SetColor("_EmissionColor", emissionColor);

        Debug.Log("🚨 버튼 작동! 딜레이 시작...");

        // 4. 기다리기
        yield return new WaitForSeconds(delayTime);

        // 5. 연결된 다음 행동 실행 (예: 문 열기, 엔딩 크레딧)
        onSequenceFinish?.Invoke();
    }
}