using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.Events;

public class KnobWaterActivator : MonoBehaviour
{
    public UnityEvent Event;

    public XRKnob knob;                         // XRKnob 참조
    public FaucetWaterController waterControl;  // 물 스크립트

    public float triggerValue = 1f;             // value가 이 이상이면 물 켜짐

    private void Start()
    {
        // XRKnob의 값 변경 이벤트 연결
        knob.onValueChange.AddListener(OnKnobValueChanged);
    }

    private void OnKnobValueChanged(float val)
    {
        // 값이 충분히 올라가면 물 ON
        if (val >= triggerValue)
        {
            waterControl.StartWater();
            Event.Invoke();
        }
        else
        {
            waterControl.StopWater();
        }
    }
}
