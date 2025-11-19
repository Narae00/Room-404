using UnityEngine;
using UnityEngine.UIElements;

public class LeverPuzzleController : MonoBehaviour
{
    [Header("핸들 개수")]
    public int leverCount = 5;

    private HingeJoint[] joints;      // 레버가 부착되면 여기에 등록됨
    private bool[] attached;          // 부착 여부
    private float[] angles;           // 각 레버 현재 각도
    private bool[] leverOn;
    private bool puzzelClear = false;
    void Awake()
    {
        joints = new HingeJoint[leverCount];
        attached = new bool[leverCount];
        angles = new float[leverCount];
        leverOn = new bool[leverCount];
    }

    void Update()
    {
        for (int i = 0; i < leverCount; i++)
        {
            if (!attached[i] || joints[i] == null) continue;

            // hinge 각도 읽기
            angles[i] = joints[i].angle;
            
            if (angles[i] > 50f)
            {
                leverOn[i] = true;
            }
            else
            {
                leverOn[i] = false;
            }
        }

        // 전체 레버 연동 1번 레버 이동 -> 2, 5번 레버 이동, 2번 레버 이동 -> 1, 3번 레버 이동
        // 반복문 없이 다른 방법 사용 가능 할 수도
        for (int i = 0; i < leverCount; i++)
        {
            if (!attached[i] || joints[i] == null) continue;

            int left  = (i - 1 + leverCount) % leverCount;
            int right = (i + 1) % leverCount;

            if (attached[left] && joints[left] != null)
            {
                SetLeverAngle(left, leverOn[i] ? -70f : 70f);
            }

            if (attached[right] && joints[right] != null)
            {
                SetLeverAngle(right, leverOn[i] ? -70f : 70f);
            }
        }

        CheckPuzzle();
    }

    public void SetLeverAngle(int index, float targetAngle)
    {
        if (joints[index] == null) return;

        var hinge = joints[index];
        hinge.useSpring = true;

        var spring = hinge.spring;
        spring.spring = 60;   // 적당한 스프링 값
        spring.damper = 25;
        spring.targetPosition = targetAngle;
        hinge.spring = spring;

        Debug.Log($"레버 {index} → 목표 각도 {targetAngle}°");
    }

    public void RegisterLever(int index, HingeJoint hinge)
    {
        joints[index] = hinge;
        attached[index] = true;
        Debug.Log($"Lever {index} 등록 완료!");
    }

    public void CheckPuzzle()
    {   
        if (!puzzelClear)
        {
            int count = 0;
            for (int i = 0; i < leverCount; i++)
            {
                if (leverOn[i]) count++;
            }

            if (count == leverCount)
            {
                Debug.Log("🎉 퍼즐 성공!");
                puzzelClear = true;
            }
        }
        
    }

}
