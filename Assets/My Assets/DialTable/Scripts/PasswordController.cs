using UnityEngine;
using UnityEngine.Events;

public class PasswordController : MonoBehaviour
{
    [Header("4자리 비밀번호")]
    public DialButtonController[] numbers;

    [Header("비밀번호")]
    public int[] password = new int[] { 1, 2, 3, 4 };

    [Header("정답 이벤트")]
    public UnityEvent OnCorrectPassword;
    public UnityEvent OnIncorrectPassword;

    [Header("정답 시 서랍 Animator")]
    public Animator drawerAnimator;

    private bool isUnlocked = false;

    public void CheckPassword()
    {
        if (isUnlocked || numbers.Length != password.Length) return;

        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i].GetCurrentNumber() != password[i])
            {
                Debug.Log("틀린 비밀번호");
                OnIncorrectPassword?.Invoke();
                return;
            }
        }

        Debug.Log("정답");
        isUnlocked = true;

        // ✅ 애니메이션 트리거 실행
        if (drawerAnimator != null)
            drawerAnimator.SetTrigger("OpenTrigger");
    }
}
