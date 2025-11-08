using UnityEngine;
using UnityEngine.InputSystem;

public class Inputtest : MonoBehaviour
{
    public InputActionProperty testActionValue;
    public InputActionProperty testActionButton;

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float value = testActionValue.action.ReadValue<float>();
        Debug.Log("value : " + value);

        bool button = testActionButton.action.IsPressed();
        Debug.Log("button :" + button);
    }
}
