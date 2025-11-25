using UnityEngine;

public class DropItem : MonoBehaviour
{
    private Rigidbody rb;

    [Header("사운드 설정")]
    public AudioSource audioSource; // 소리 나는 스피커
    public AudioClip dropSound;     // "철컥" or "퉁" 소리 파일

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // 오디오 소스가 비어있으면 내 몸에 있는 거 찾아쓰기
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void DropNow()
    {
        // 1. 일단 눈에 보이게 켜기 (숨겨져 있었다면)
        gameObject.SetActive(true);

        // 2. 물리 고정 해제 -> 툭 떨어짐!
        if (rb != null)
        {
            rb.isKinematic = false; 
        }

        // 3. ★ 소리 재생 (추가됨)
        if (audioSource != null && dropSound != null)
        {
            audioSource.PlayOneShot(dropSound);
        }
        
        Debug.Log("🎁 레이저 포인터 등장, 투하, 소리 재생 완료!");
    }
}