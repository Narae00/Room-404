using UnityEngine;
using System;

public class LeverPuzzleController : MonoBehaviour
{
    public int leverCount = 5;

    // 레버 상태 저장
    [SerializeField] private bool[] leverStates;

    public event Action OnPuzzleSolved;
    public event Action<int, bool> OnLeverChanged;

    private bool isSolved = false;

    private void Awake()
    {
        leverStates = new bool[leverCount];
    }

    private void Start()
    {
        ResetPuzzle();
    }

    /// 모든 레버 초기화
    public void ResetPuzzle()
    {
        for (int i = 0; i < leverCount; i++)
        {
            leverStates[i] = false;
            OnLeverChanged?.Invoke(i, false);
        }

        Debug.Log("🔄 퍼즐 초기화 완료");
    }

    /// 모든 레버 ON이면 성공
    private bool CheckSolved()
    {
        for (int i = 0; i < leverCount; i++)
        {
            if (!leverStates[i]) return false; // 하나라도 false면 실패
        }
        return true;
    }

    public void SetLeverState(int leverIndex, bool state)
    {
        if (leverIndex < 0 || leverIndex >= leverCount)
            return;
        if (isSolved) return;


        leverStates[leverIndex] = state;

        // 이벤트
        OnLeverChanged?.Invoke(leverIndex, state);

        // 퍼즐 성공 검사
        if (CheckSolved())
        {
            isSolved = true;
            Debug.Log("🎉 퍼즐 성공!");
            OnPuzzleSolved?.Invoke();
        }
    }

    public bool GetLeverState(int index)
    {
        return leverStates[index];
    }
}
