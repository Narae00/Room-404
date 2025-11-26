using UnityEngine;
using System.Collections; // 코루틴(시간차 실행) 사용을 위해 추가

public class KeyFirstGrab : MonoBehaviour
{
    [Header("타겟 설정")]
    public Light targetLight;           // 💡 켜질 조명
    public AudioSource audioSource;    // 🔊 소리 재생기

    [Header("사운드 파일")]
    public AudioClip keyGrabSound;      // 1. 열쇠 집는 소리 (즉시)
    public AudioClip lightOnSound;      // 3. 전등 켜지는 소리 (딜레이 후)

    [Header("타이밍 설정")]
    public float delayTime = 0.5f;     // ⏳ 딜레이 시간 (0.5초)
    [Range(0.1f, 1f)]
    public float lightOnSoundVolume = 0.8f; // 전등 소리 볼륨 (0.8이 기본)

    private bool isTriggered = false; // 이미 작동했는지 체크하는 변수

    void Start()
    {
        // 게임 시작 시 조명을 꺼두는 것이 안전합니다.
        if (targetLight != null)
        {
            targetLight.gameObject.SetActive(false);
        }
    }

    // 이 함수를 XR Grab Interactable의 Select Entered에 연결하세요 (변함없음)
    public void OnFirstGrab()
    {
        // 만약 이미 작동했다면(true라면) -> 아무것도 안 하고 리턴!
        if (isTriggered) return;

        // 3. "작동했음"으로 상태 변경 (이제 다시는 실행 안 됨)
        isTriggered = true;

        // 시간차를 두고 실행하는 코루틴 시작
        StartCoroutine(GrabSequenceRoutine());
    }

    // [새로운 시퀀스 함수]: 시간차와 순서를 제어합니다.
    IEnumerator GrabSequenceRoutine()
    {
        // 1. [소리 1] 열쇠 집는 소리 즉시 재생
        if (audioSource != null && keyGrabSound != null)
        {
            audioSource.PlayOneShot(keyGrabSound);
        }

        // 2. [딜레이] 지정된 시간만큼 대기 (0.5초)
        yield return new WaitForSeconds(delayTime);

        // 3. [조명 켜기] 조명 오브젝트 활성화
        if (targetLight != null)
        {
            targetLight.gameObject.SetActive(true);
        }

        // 4. [소리 2] 전등 켜지는 소리 재생 (3번과 동시에 발생)
        if (audioSource != null && lightOnSound != null)
        {
            // 지정된 볼륨(lightOnSoundVolume)으로 소리 재생
            audioSource.PlayOneShot(lightOnSound, lightOnSoundVolume);
        }
    }
}