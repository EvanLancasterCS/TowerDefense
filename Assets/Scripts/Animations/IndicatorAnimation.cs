using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform[] arrows = new RectTransform[4];
    private Coroutine[] arrowsAnims = new Coroutine[4];
    private float arrowDistance = 100;
    private float blinkRate = 1f;

    public void SetIndicator(int dir, Vector3 worldPos)
    {
        if(dir >= 0 && dir < 4)
        {
            StopIndicator(dir);
            Coroutine anim = StartCoroutine(AnimateArrow(dir, worldPos));
            arrowsAnims[dir] = anim;
        }
    }

    public IEnumerator AnimateArrow(int index, Vector3 worldPos)
    {
        RectTransform myArrow = arrows[index];
        Image myArrowImage = myArrow.GetComponent<Image>();
        float currAlpha = 1;
        bool increasing = false;
        myArrow.gameObject.SetActive(true);
        while(true)
        {
            // get 2 dimensional difference between cam and spawn,
            // then normalize and multiply with the arrow distance for UI pos
            // i don't think we can get around normalizing even tho normalizing is cringe
            Vector3 camPos = Camera.main.transform.position;
            Vector2 diff = new Vector2(worldPos.x, worldPos.z) - new Vector2(camPos.x, camPos.z);
            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

            diff.Normalize();

            Vector2 arrowPos = diff * arrowDistance;
            myArrow.anchoredPosition = arrowPos;
            myArrow.eulerAngles = new Vector3(0, 0, angle - 90);

            // blink alpha
            float alphaRate = Time.deltaTime / blinkRate;
            if (increasing)
                currAlpha += alphaRate;
            else
                currAlpha -= alphaRate;

            if (currAlpha <= 0)
            {
                increasing = true;
                currAlpha = 0;
            }
            else if(currAlpha >= 1)
            {
                increasing = false;
                currAlpha = 1;
            }
            Color c = myArrowImage.color;
            myArrowImage.color = new Color(c.r, c.g, c.b, currAlpha);

            yield return null;
        }
    }

    private void StopIndicator(int i)
    {
        if (arrowsAnims[i] != null)
        {
            StopCoroutine(arrowsAnims[i]);
            arrowsAnims[i] = null;
        }

        arrows[i].gameObject.SetActive(false);
    }

    public void StopIndicators()
    {
        for(int i = 0; i < arrows.Length; i++)
        {
            StopIndicator(i);
        }
    }
}
