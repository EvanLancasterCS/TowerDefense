using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DifficultyNotificationAnimation : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private TextMeshProUGUI textUI;
    private float animTime = 5f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        textUI = GetComponent<TextMeshProUGUI>();
    }

    public void Animate(string text, Color c)
    {
        float half = animTime / 2f;
        textUI.text = text;
        textUI.color = c;
        StartCoroutine(Animate(half));
    }

    private IEnumerator Animate(float half)
    {
        float cTime = 0;
        while(cTime < animTime)
        {
            float alpha;
            if (cTime < half)
                alpha = 1 - ((half - cTime) / half);
            else
                alpha = (animTime - cTime) / half;

            canvasGroup.alpha = alpha;
            cTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}
