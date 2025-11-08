using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class DialButtonController : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshPro numberText;
    [Range(0, 9)] public int currentNumber = 0;

    public UnityEvent OnNumberChanged;
    void Start()
    {
        UpdateDisplay();
    }

    public void IncreaseNumber()
    {
        currentNumber = (currentNumber + 1) % 10;
        UpdateDisplay();
        OnNumberChanged?.Invoke();
    }

    public void DecreaseNumber()
    {
        currentNumber = (currentNumber + 9) % 10; // -1 mod 10
        UpdateDisplay();
        OnNumberChanged?.Invoke();
    }

    void UpdateDisplay()
    {
        if (numberText)
            numberText.text = currentNumber.ToString();
    }

    public int GetCurrentNumber() => currentNumber;
}
