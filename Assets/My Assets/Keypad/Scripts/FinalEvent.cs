using UnityEngine;
using System.Collections;

public class FinalEvent : MonoBehaviour
{
    [Header("1. 빨간 버튼 설정")]
    public Transform redButtonModel; // 눌려질 버튼 모델
    public AudioSource buttonSource; // 버튼 소리 스피커
    public AudioClip clickSound;     // "딸각" 소리

    [Header("2. 책상 이동 설정")]
    public Transform desk;           // 움직일 책상
    public Vector3 deskMoveDir = new Vector3(2, 0, 0); // 이동할 방향과 거리
    public float deskSpeed = 1.0f;   // 이동 속도
    public AudioSource deskSource;   // 책상 소리 스피커
    public AudioClip deskDragSound;  // "드르륵" 끄는 소리
    public float deskSoundDelay = 0.5f; // 버튼 누르고 몇 초 뒤에 책상 소리?

    [Header("3. 탈출 문 설정")]
    public Transform exitDoor;       // 탈출 문 (회전할 축/부모)
    public float doorOpenAngle = 90f; // 문 열릴 각도
    public AudioSource doorSource;   // 문 소리 스피커
    public AudioClip doorOpenSound;  // "끼이익" 소리
    public float doorDelay = 3.0f;   // 책상 움직이고 몇 초 뒤에 문 열리나?

    private bool isActivated = false;

    // ★ 빨간 버튼을 눌렀을 때(XR Simple Interactable) 이 함수를 연결하세요!
    public void OnRedButtonPressed()
    {
        if (isActivated) return;
        isActivated = true;

        StartCoroutine(SequenceRoutine());
    }

    IEnumerator SequenceRoutine()
    {
        // --- [1단계: 버튼 눌림] ---
        // 버튼 모델을 살짝 아래로 내림 (눌리는 연출)
        if (redButtonModel != null)
            redButtonModel.localPosition += new Vector3(0, -0.02f, 0);

        if (buttonSource != null)
            buttonSource.PlayOneShot(clickSound); // "딸각"

        // --- [2단계: 책상 이동] ---
        yield return new WaitForSeconds(deskSoundDelay); // 딜레이

        if (deskSource != null)
            deskSource.PlayOneShot(deskDragSound); // "드르륵~"

        // 책상 이동 로직
        Vector3 startPos = desk.position;
        Vector3 endPos = desk.position + deskMoveDir;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * deskSpeed;
            desk.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // --- [3단계: 문 열림] ---
        yield return new WaitForSeconds(doorDelay); // 책상 다 밀리고 잠시 대기

        if (doorSource != null)
            doorSource.PlayOneShot(doorOpenSound); // "끼이익"

        // 문 회전 로직
        Quaternion startRot = exitDoor.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, doorOpenAngle, 0);
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2.0f; // 문은 좀 빨리 열림
            exitDoor.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
    }
}