using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SprayCan : MonoBehaviour
{
    public Transform nozzle; // 발사구 위치
    public ParticleSystem sprayParticle; // 스프레이 파티클
    public float range = 2.0f; // 스프레이 닿는 거리
    public float revealSpeed = 0.5f; // 얼마나 빨리 나타나게 할지

    private bool isSpraying = false;

    // XR Grab Interactable의 이벤트에 연결할 함수들
    public void StartSpray()
    {
        isSpraying = true;
        sprayParticle.Play();
        // 여기에 '치이익' 사운드 재생 코드 추가 가능
    }

    public void StopSpray()
    {
        Debug.Log("🔥 발사 신호 받음! 🔥");  // <--- 이 줄 추가
        isSpraying = false;
        sprayParticle.Stop();
    }

    void Update()
    {
        // 뗄 때 끄기 (★이게 있어야 멈춥니다!)
    if (Input.GetKeyUp(KeyCode.Space)) 
    {
        StopSpray();
    }
        if (Input.GetKeyDown(KeyCode.Space)) StartSpray();
        if (isSpraying)
        {
            // 1. 노즐 앞쪽으로 투명 레이저 발사
            if (Physics.Raycast(nozzle.position, nozzle.forward, out RaycastHit hit, range))
            {
                // 2. 맞은 물체가 '숨겨진 단서(HiddenClue)'라면?
                if (hit.collider.CompareTag("HiddenClue"))
                {
                    // 3. 그 물체의 재질을 가져와서 투명도(Alpha)를 높임
                    MeshRenderer rend = hit.collider.GetComponent<MeshRenderer>();
                    if (rend != null)
                    {
                        Color col = rend.material.color;
                        col.a += revealSpeed * Time.deltaTime; // 점점 불투명하게
                        col.a = Mathf.Clamp01(col.a); // 1을 넘지 않게
                        rend.material.color = col;
                    }
                }
            }
        }
    }
}