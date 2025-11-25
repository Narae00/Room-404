using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EndingZone : MonoBehaviour
{   
    public FadeScreen fadeScreen;
    public float delayTime = 1f;

    private bool isEnd = false;
    
    private void OnTriggerEnter(Collider other)
    {   
        if (isEnd) return ;

        if (other.CompareTag("Player"))
        {
            isEnd = true;
            Debug.Log("탈출 성공");
            StartCoroutine(RoadStartScene());
        }      
    }

    private IEnumerator RoadStartScene()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSecondsRealtime(delayTime);

        SceneManager.LoadScene(0);
    }
}
