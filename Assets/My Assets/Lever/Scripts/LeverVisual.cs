using UnityEngine;

public class LeverVisual : MonoBehaviour
{
    public int leverIndex;        // 0~4
    public LeverPuzzleControll controller;

    public float offAngle = -40f; // OFF 상태 각도
    public float onAngle  = 40f;  // ON 상태 각도

    private HingeJoint hinge;
    private bool attached = false;

    private void Start()
    {
        controller.OnLeverChanged += OnLeverStateChanged;
    }

    private void Update()
    {
        // AttachTrigger 에 의해 hinge가 생기면 참조 확보
        if (!attached)
        {
            hinge = GetComponent<HingeJoint>();
            if (hinge != null)
            {
                attached = true;
                // attach 직후 현재 상태를 반영
                bool initialState = controller.GetLeverState(leverIndex);
                ApplyTargetAngle(initialState);
            }
        }
    }

    private void OnLeverStateChanged(int index, bool isOn)
    {
        if (!attached) return;
        if (index != leverIndex) return;

        ApplyTargetAngle(isOn);
    }

    private void ApplyTargetAngle(bool isOn)
    {
        if (hinge == null) return;

        var spring = hinge.spring;
        spring.targetPosition = isOn ? onAngle : offAngle;
        hinge.spring = spring;
    }

    
}
