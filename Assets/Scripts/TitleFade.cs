using System.Collections;
using TMPro;
using UnityEngine;

public class TitleFade : MonoBehaviour
{
    TextMeshProUGUI  text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(FadeTextToFullAlpha());
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    public IEnumerator FadeTextToFullAlpha()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + Time.deltaTime / 2.0f);
            yield return null;
        }

        StartCoroutine(FadeTextToZeroAlpha());
    }

    public IEnumerator FadeTextToZeroAlpha()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime / 2.0f);
            yield return null;
        }

        StartCoroutine(FadeTextToFullAlpha());
    }
}