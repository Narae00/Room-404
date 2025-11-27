using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MirrorText : MonoBehaviour
{
    [SerializeField] Material material;

    private void Awake()
    {
        material.color = new Color(1, 1, 1, 0);
    }
    public void ShowMirrorText()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(2f);
        while(material.color.a <= 0.8f)
        {
            Color prevColor = material.color;
            material.color = new Color(prevColor.r, prevColor.g, prevColor.b, prevColor.a += Time.deltaTime / 10f);
            yield return new WaitForEndOfFrame();
        }
    }

}
