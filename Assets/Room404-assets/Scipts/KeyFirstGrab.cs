using UnityEngine;

public class KeyFirstGrab : MonoBehaviour
{
    public Light targetLight; // 켜질 조명
    public AudioSource audioSource; // 소리
    
    private bool isTriggered = false; // 이미 작동했는지 체크하는 변수

    // 이 함수를 XR Grab Interactable의 Select Entered에 연결하세요
    public void OnFirstGrab()
    {
        // 만약 이미 작동했다면(true라면) -> 아무것도 안 하고 리턴!
        if (isTriggered) return;

        // 1. 조명 켜기
        if (targetLight != null) targetLight.gameObject.SetActive(true);

        // 2. 소리 재생
        if (audioSource != null) audioSource.Play();

        // 3. "작동했음"으로 상태 변경 (이제 다시는 실행 안 됨)
        isTriggered = true;
    }
}