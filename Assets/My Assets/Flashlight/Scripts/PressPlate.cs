using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 필수

public class PressPlate : MonoBehaviour
{
    [Header("Plate Settings")]
    public Transform plateMesh;
    public Vector3 downOffset = new Vector3(0, -0.05f, 0);
    public float moveSpeed = 5f;

    [Header("Detector Settings")]
    public Transform detector;
    public Vector3 rotateAmount = new Vector3(0, 90f, 0);
    public float rotateSpeed = 90f;

    [Header("Glow Settings")]
    public Renderer lensRenderer;
    public int materialIndex = 1;
    [ColorUsage(true, true)]
    public Color onColor = Color.red * 3f;
    public Color offColor = Color.black;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip rotateSound;
    public float soundDuration = 1.0f; // ★ 소리 재생 시간 (1초)

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isPressed = false;

    private Quaternion initialRot;
    private Quaternion targetRot;
    
    private Coroutine soundCoroutine; // 실행 중인 타이머 저장용

    void Start()
    {
        startPos = plateMesh.position;
        initialRot = detector.localRotation;
        targetRot = initialRot * Quaternion.Euler(rotateAmount);
        targetPos = startPos;

        SetGlow(offColor);
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsValidTrigger(other))
        {
            isPressed = true;
            targetPos = startPos + downOffset;
            SetGlow(onColor);

            // ★ 소리 재생 (타이머 시작)
            PlaySoundForSeconds(soundDuration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsValidTrigger(other))
        {
            isPressed = false;
            targetPos = startPos;
            SetGlow(offColor);

            // ★ 소리 재생 (타이머 시작)
            PlaySoundForSeconds(soundDuration);
        }
    }

    // 조건 체크 함수 (코드가 너무 길어져서 따로 뺌)
    bool IsValidTrigger(Collider other)
    {
        return other.CompareTag("Player") || 
               other.GetComponent<Rigidbody>() != null || 
               other.GetComponent<CharacterController>() != null;
    }

    // ★ 핵심: 정해진 시간만큼만 틀고 끄는 함수
    void PlaySoundForSeconds(float duration)
    {
        if (audioSource == null || rotateSound == null) return;

        // 이미 돌고 있는 타이머가 있다면 취소하고 새로 시작
        if (soundCoroutine != null) StopCoroutine(soundCoroutine);
        
        soundCoroutine = StartCoroutine(SoundRoutine(duration));
    }

    IEnumerator SoundRoutine(float duration)
    {
        audioSource.clip = rotateSound;
        audioSource.Play(); // 재생 시작

        yield return new WaitForSeconds(duration); // 설정한 시간만큼 대기

        audioSource.Stop(); // 강제 정지
    }

    void Update()
    {
        plateMesh.position = Vector3.Lerp(plateMesh.position, targetPos, Time.deltaTime * moveSpeed);

        if (isPressed)
            detector.localRotation = Quaternion.RotateTowards(detector.localRotation, targetRot, rotateSpeed * Time.deltaTime);
        else
            detector.localRotation = Quaternion.RotateTowards(detector.localRotation, initialRot, rotateSpeed * Time.deltaTime);
    }

    void SetGlow(Color targetColor)
    {
        if (lensRenderer != null)
            lensRenderer.materials[materialIndex].SetColor("_EmissionColor", targetColor);
    }
}