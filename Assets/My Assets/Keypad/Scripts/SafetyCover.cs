using UnityEngine;

public class SafetyCover : MonoBehaviour
{
    [Header("설정")]
    public float openAngle = -120f; // 뚜껑이 열릴 각도 (X축 기준)
    public float speed = 2.0f;

    [Header("사운드")]
    public AudioSource audioSource;
    public AudioClip openSound; // "징~" 하는 기계음

    private bool isOpen = false;

    // 키패드 성공 시 이 함수를 호출하세요
    public void OpenCover()
    {
        if (isOpen) return;
        isOpen = true;

        // 소리 재생
        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound);

        // 열기 시작
        StartCoroutine(RotateRoutine());
    }

    System.Collections.IEnumerator RotateRoutine()
    {
        Quaternion startRot = transform.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(openAngle, 0, 0); // X축 회전

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
    }
}