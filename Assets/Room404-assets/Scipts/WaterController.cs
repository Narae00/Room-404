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
    public ParticleSystem steamParticle;
    public float steamDuration = 3f;
    public MirrorText mirrorText;

    private bool isRunning = false;
    private float waterTimer = 0f;

    private bool steamPlayed = false;
    private float steamTimer = 0f;

    void Update()
    {
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
                // 물이 다 차면 연기 시작
                if (!steamPlayed)
                {
                    steamPlayed = true;
                    steamTimer = 0f;

                    steamParticle.gameObject.SetActive(true);
                    steamParticle.Clear();
                    steamParticle.Play();
                }
            }

            if (waterTimer >= waterDuration)
            {
                StopWater();
            }
        }

        // 연기 타이머
        if (steamPlayed)
        {
            steamTimer += Time.deltaTime;

            // 거울 글씨 표시
            mirrorText.ShowMirrorText();

            if (steamTimer >= steamDuration)
            {
                steamParticle.Stop();
                // SetActive(false) 하지 않음 — 꺼지면 다시 Play가 안 먹히기 때문
                steamPlayed = false;
            }
        }
    }

    public void StartWater()
    {
        if (isRunning) return;

        isRunning = true;
        waterTimer = 0f;

        // 연기 초기화
        steamPlayed = false;
        steamTimer = 0f;

        // 파티클 오브젝트는 끄지 않음 → 재생 가능상태 유지
        steamParticle.Stop();
        steamParticle.Clear();

        if (!waterParticle.isPlaying)
            waterParticle.Play();
    }

    public void StopWater()
    {
        isRunning = false;

        if (waterParticle.isPlaying)
            waterParticle.Stop();
    }
}
