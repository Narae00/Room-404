using UnityEngine;

public class PressurePlateRotateDetector : MonoBehaviour
{
    [Header("Plate Settings")]
    public Transform plateMesh;
    public Vector3 downOffset = new Vector3(0, -0.05f, 0);
    public float moveSpeed = 5f;

    [Header("Detector Settings")]
    public Transform detector;                 // 회전할 감지기 오브젝트
    public Vector3 rotateAmount = new Vector3(0, 90f, 0); // 회전할 총 각도
    public float rotateSpeed = 90f;            // 초당 회전 속도

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isPressed = false;

    private Quaternion initialRot;
    private Quaternion targetRot;

    void Start()
    {
        startPos = plateMesh.position;
        initialRot = detector.localRotation;
        targetRot = initialRot * Quaternion.Euler(rotateAmount);
        targetPos = startPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        isPressed = true;
        targetPos = startPos + downOffset;
    }

    private void OnTriggerExit(Collider other)
    {
        isPressed = false;
        targetPos = startPos;
    }

    void Update()
    {
        // Plate movement
        plateMesh.position = Vector3.Lerp(
            plateMesh.position,
            targetPos,
            Time.deltaTime * moveSpeed
        );

        // Detector rotation logic
        if (isPressed)
        {
            // Plate 눌리는 동안 detector를 목표 각도까지 회전
            detector.localRotation = Quaternion.RotateTowards(
                detector.localRotation,
                targetRot,
                rotateSpeed * Time.deltaTime
            );
        }
        else
        {
            // Plate에서 벗어나면 detector를 원래 각도로 역회전
            detector.localRotation = Quaternion.RotateTowards(
                detector.localRotation,
                initialRot,
                rotateSpeed * Time.deltaTime
            );
        }
    }
}
