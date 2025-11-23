using UnityEngine;

public class LeverLightController : MonoBehaviour
{
    public LeverPuzzleController controller;
    public int targetLeverIndex = 0; // 0번 레버 감지
    public Light targetLight;

    private void Start()
    {
        // 레버 상태 변할 때마다 실행
        controller.OnLeverChanged += HandleLeverChanged;

        // 시작할 때 현재 상태 반영
        HandleLeverChanged(targetLeverIndex, controller.GetLeverState(targetLeverIndex));
    }

    private void HandleLeverChanged(int index, bool state)
    {
        if (index != targetLeverIndex) return;

        targetLight.enabled = state;
    }
}
