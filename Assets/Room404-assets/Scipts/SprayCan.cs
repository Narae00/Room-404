using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SprayCan : MonoBehaviour
{
    public Transform nozzle;
    public ParticleSystem sprayParticle;
    public float range = 2.0f;
    public float revealSpeed = 0.5f;

    [Header("사운드 설정 (추가됨)")]
    public AudioSource audioSource; // 스피커

    private bool isSpraying = false;

    public void StartSpray()
    {
        isSpraying = true;
        if (sprayParticle != null) sprayParticle.Play();

        // ★ 소리 재생 (이미 재생 중이면 다시 틀지 않음)
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopSpray()
    {
        isSpraying = false;
        if (sprayParticle != null) sprayParticle.Stop();

        // ★ 소리 정지
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    void Update()
    {
        // (테스트용 스페이스바 코드 - 필요 없으면 지워도 됨)
        if (Input.GetKeyDown(KeyCode.Space)) StartSpray();
        if (Input.GetKeyUp(KeyCode.Space)) StopSpray();

        if (isSpraying)
        {
            // ... (기존 레이캐스트 로직 유지) ...
            if (Physics.Raycast(nozzle.position, nozzle.forward, out RaycastHit hit, range))
            {
                if (hit.collider.CompareTag("HiddenClue"))
                {
                    MeshRenderer rend = hit.collider.GetComponent<MeshRenderer>();
                    if (rend != null)
                    {
                        Color col = rend.material.color;
                        col.a += revealSpeed * Time.deltaTime;
                        col.a = Mathf.Clamp01(col.a);
                        rend.material.color = col;
                    }
                }
            }
        }
    }
}