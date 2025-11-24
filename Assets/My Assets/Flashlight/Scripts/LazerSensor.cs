using UnityEngine;
using UnityEngine.Events;

public class LaserSensor : MonoBehaviour
{
    [Header("감지 설정")]
    public float requiredHitTime = 1.0f;   // 몇 초 동안 레이저가 닿아야 하는지
    public float decayPerSec = 0.5f;       // 끊기면 줄어드는 속도
    public UnityEvent onUnlocked;          // 이벤트 (예: 문 열기)

    private float _charge = 0f;
    private bool _hitThisFrame = false;
    private bool solvePuzzle = false;

    void Update()
    {   
        if (solvePuzzle) return ;

        // 매 프레임 감지 상태 갱신
        if (_hitThisFrame)
        {
            _charge += Time.deltaTime;
            Debug.Log(_charge);
            if (_charge >= requiredHitTime)
            {
                solvePuzzle = true;
                _charge = requiredHitTime;
                onUnlocked.Invoke(); // 이벤트 실행
                Debug.Log("레이저 감지 완료");
            }
        }
        else
        {
            _charge = Mathf.Max(0, _charge - decayPerSec * Time.deltaTime);
        }

        _hitThisFrame = false;
    }

    // LaserPointer에서 이 함수를 호출
    public void RegisterHit()
    {
        _hitThisFrame = true;
    }
}
