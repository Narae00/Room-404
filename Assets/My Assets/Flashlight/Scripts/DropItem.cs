using UnityEngine;

public class DropItem : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // 내 몸에 있는 리지드바디 가져오기
    }

    // 이 함수를 레버가 다 내려갔을 때 실행하면 됩니다!
    public void DropNow()
    {
        rb.isKinematic = false; // 고정 해제 -> 툭 떨어짐!
        Debug.Log("레이저 포인터 투하!");
    }
}