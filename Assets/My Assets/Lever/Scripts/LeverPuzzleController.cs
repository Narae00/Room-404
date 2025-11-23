using UnityEngine;
using System;

public class LeverPuzzleControll : MonoBehaviour
{
    // 레버 상태 (false=0, true=1)
    [SerializeField] private bool[] leverStates = new bool[5];

    // 퍼즐이 완성됐을 때 호출되는 이벤트
    public event Action OnPuzzleSolved;
    public event Action<int, bool> OnLeverChanged; 

    private void Start()
    {
        ResetPuzzle();
    }

    /// <summary>
    /// 외부에서 레버를 당길 때 호출하는 함수
    /// XRGrabInteractable or Lever script에서 이 함수 호출하면 됨
    /// leverIndex: 0~4 (실제 레버 번호-1)
    /// </summary>
    public void ToggleLever(int leverIndex)
    {
        if (leverIndex < 0 || leverIndex >= 5)
        {
            Debug.LogError("Lever index out of range.");
            return;
        }

        // 실제 토글 규칙 수행
        ApplyToggleRule(leverIndex);

        // 현재 상태 확인용
        Debug.Log("Lever State: " + GetStateString());

        // 퍼즐 성공 조건 검사
        if (CheckSolved())
        {
            Debug.Log("Puzzle Solved!");
            OnPuzzleSolved?.Invoke();
        }
    }

    /// <summary>
    /// 레버 토글 규칙 적용
    /// </summary>
    private void ApplyToggleRule(int i)
    {

    }

    /// <summary>
    /// 퍼즐 초기화
    /// </summary>
    public void ResetPuzzle()
    {

    }

    /// <summary>
    /// 퍼즐이 성공했는지 확인 (전체 1인지 여부)
    /// </summary>
    private bool CheckSolved()
    {
        for (int i = 0; i < 5; i++)
        {
            if (!leverStates[i]) return false;
        }
        return true;
    }

    /// <summary>
    /// 디버그 출력용 상태 스트링
    /// </summary>
    private string GetStateString()
    {
        return $"{(leverStates[0] ? 1 : 0)} " +
               $"{(leverStates[1] ? 1 : 0)} " +
               $"{(leverStates[2] ? 1 : 0)} " +
               $"{(leverStates[3] ? 1 : 0)} " +
               $"{(leverStates[4] ? 1 : 0)}";
    }

    public bool GetLeverState(int index)
    {
        return leverStates[index];
    }
}
