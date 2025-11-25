using UnityEngine;

public class FaucetWaterController : MonoBehaviour
{
    [Header("Water Settings")]
    public ParticleSystem waterParticle;      
    public Transform waterPlane;              
    public float waterRiseSpeed = 0.1f;       
    public float maxWaterHeight = 0.3f;       

    [Header("Duration Settings")]
    public float waterDuration = 5f;          

    [Header("Effects")]
    public ParticleSystem steamParticle;       // 연기 파티클
    public float steamDuration = 3f;           // 연기 재생 시간

    private bool isRunning = false;
    private float waterTimer = 0f;

    private bool steamPlayed = false;
    private float steamTimer = 0f;

    void Update()
    {
        // 물 재생 중
        if (isRunning)
        {
            waterTimer += Time.deltaTime;

            // 물 채우기
            if (waterPlane.localPosition.y < maxWaterHeight)
            {
                waterPlane.localPosition += Vector3.up * waterRiseSpeed * Time.deltaTime;
            }
            else
            {
                // 물이 다 찼으면 연기 파티클 ON
                if (!steamPlayed)
                {   
                    steamParticle.Clear();
                    steamParticle.gameObject.SetActive(true);
                    steamParticle.Play();
                    steamPlayed = true;
                    steamTimer = 0f;   // 연기 타이머 초기화
                }
            }

            // 물 재생 시간 끝
            if (waterTimer >= waterDuration)
            {
                StopWater();
            }
        }

        // 연기 재생 중이면 타이머 계산
        if (steamPlayed)
        {
            steamTimer += Time.deltaTime;

            // 연기 지속 시간 끝나면 끄기
            if (steamTimer >= steamDuration)
            {
                steamParticle.Stop();
                steamPlayed = false;
            }
        }
    }

    // 수도꼭지 ON
    public void StartWater()
    {
        if (isRunning) return;

        isRunning = true;
        waterTimer = 0f;

        // 연기 초기화
        steamPlayed = false;
        steamTimer = 0f;
        steamParticle.Stop();
        steamParticle.Clear();
        steamParticle.gameObject.SetActive(false);

        if (!waterParticle.isPlaying)
            waterParticle.Play();
    }

    // 수도꼭지 OFF
    public void StopWater()
    {
        isRunning = false;

        if (waterParticle.isPlaying)
            waterParticle.Stop();
    }
}
