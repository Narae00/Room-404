using UnityEngine;

public class bookshelfController : MonoBehaviour
{
    public Animator bookshelfAnimator;
    public string triggerName = "Open";
    public string[] correctPattern;
    private string[] slotStates;
    private bool opened = false;

    void Start()
    {
        slotStates = new string[correctPattern.Length];
        for (int i = 0; i < slotStates.Length; i++)
            slotStates[i] = "";
    }

    public void UpdateSlot(int index, string bookTag)
    {
        slotStates[index] = bookTag;
        CheckPattern();
    }

    private void CheckPattern()
    {
        if(opened) return ;

        for (int i = 0; i < correctPattern.Length; i++)
        {
            if (slotStates[i] != correctPattern[i]) return ;
        }

        Debug.Log("책장 퍼즐 완료 -> 책장 애니메이션 재생");
        bookshelfAnimator.SetTrigger(triggerName);
        opened = true; 
    }
}