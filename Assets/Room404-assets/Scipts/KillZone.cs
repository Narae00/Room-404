using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour
{
    public FadeScreen fadeScreen;
    public float delayTime = 1f;

    private bool isKill = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isKill) return ;

        if (other.CompareTag("Player"))
        {
            isKill = true;
            Debug.Log("유저 충돌 !!!");
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
