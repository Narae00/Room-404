using UnityEngine;
using TMPro; // 텍스트메쉬프로(글자) 사용
using UnityEngine.Events; // 이벤트(문 열기) 사용

public class SafeKeypad : MonoBehaviour
{
    [Header("설정")]
    public string correctPassword = "1234"; // 정답 비밀번호
    public TMP_Text displayScreen; // 숫자가 뜰 화면 (텍스트)
    public int maxDigits = 4; // 최대 자릿수

    [Header("이벤트")]
    public UnityEvent onPasswordCorrect; // 맞았을 때 실행 (문 열기)
    public UnityEvent onPasswordWrong;   // 틀렸을 때 실행 (소리 등)

    private string currentInput = ""; // 현재 입력된 숫자

    // 숫자 버튼을 누르면 이 함수가 실행됨
    public void InputNumber(string number)
    {
        if (currentInput.Length < maxDigits)
        {
            currentInput += number;
            UpdateDisplay();
        }
    }

    // 'Enter' 버튼 누르면 확인
    public void CheckPassword()
    {
        if (currentInput == correctPassword)
        {
            displayScreen.text = "OPEN";
            displayScreen.color = Color.green;
            onPasswordCorrect.Invoke(); // ★ 문 열기 실행!
            Debug.Log("비밀번호 정답!");
        }
        else
        {
            displayScreen.text = "ERROR";
            displayScreen.color = Color.red;
            onPasswordWrong.Invoke(); // 삑 소리 실행
            currentInput = ""; // 초기화
            Invoke("UpdateDisplay", 1.0f); // 1초 뒤에 화면 지우기
        }
    }

    // 'Clear' 버튼 누르면 지우기
    public void ClearInput()
    {
        currentInput = "";
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        displayScreen.text = currentInput;
        if(displayScreen.text != "OPEN" && displayScreen.text != "ERROR")
             displayScreen.color = Color.white;
    }
}