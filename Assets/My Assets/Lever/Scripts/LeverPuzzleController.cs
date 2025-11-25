using UnityEngine;
using System;
using UnityEngine.Events;

public class LeverPuzzleController : MonoBehaviour
{
    public int leverCount = 5;

    // 레버 상태 저장
    [SerializeField] private bool[] leverStates;
    public UnityEvent SolveLeverPuzzle;
    public event Action<int, bool> OnLeverChanged;

    // ▼▼▼ [추가된 부분 1] : 디스펜서(레이저 포인터) 연결 변수
    [Header("연결")]
    public DropItem itemDispenser;  // <-- 이렇게 고치세요!; 
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

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
            Debug.Log("🎉 레버 퍼즐 성공!");
            
            // 기존 이벤트 실행
            SolveLeverPuzzle?.Invoke();

            // ▼▼▼ [추가된 부분 2] : 아이템 투하 명령!
            if (itemDispenser != null)
            {
                itemDispenser.DropNow(); // 여기서 툭 떨어트립니다!
            }
            // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
        }
    }

    public bool GetLeverState(int index)
    {
        return leverStates[index];
    }
}