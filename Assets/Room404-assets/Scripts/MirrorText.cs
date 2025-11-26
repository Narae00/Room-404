using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MirrorText : MonoBehaviour
{
    [SerializeField] TextMeshPro tmpro;

    public void ShowMirrorText()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(2f);
        while(tmpro.color.a <= 1f)
        {
            Color prevColor = tmpro.color;
            tmpro.color = new Color(prevColor.r, prevColor.g, prevColor.b, prevColor.a += Time.deltaTime / 3);
            yield return new WaitForEndOfFrame();
        }
    }

}
