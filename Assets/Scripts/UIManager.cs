using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform inventoryArea;
    [SerializeField] AnimationCurve movementCurve;

    private List<CardUI> cards = new List<CardUI>();

    private UIAnimations anims;

    private float oddYOffset = -30f;
    private float xOffset = 90;

    private void Awake()
    {
        anims = gameObject.AddComponent<UIAnimations>();
    }

    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.K))
        {
            GameObject prefab = Instantiate(cardPrefab, inventoryArea);
            CardUI u = prefab.GetComponent<CardUI>();
            AddCard(u);
        }

    }

    void AddCard(CardUI card)
    {
        cards.Add(card);

        card.AddEvent(AnimateInventoryMouseOver, EventTriggerType.PointerEnter);
        ArrangeCards();
    }

    void ArrangeCards()
    {
        for(int i = 0; i < cards.Count; i++)
        {
            bool odd = i % 2 == 1;
            float yOff = odd ? oddYOffset : 0;
            float xOff = xOffset * i;
            int zIndex = odd ? 1 : 0;

            RectTransform cardRT = cards[i].GetComponent<RectTransform>();
            cardRT.anchoredPosition = new Vector2(xOff, yOff);
            cardRT.SetSiblingIndex(zIndex);
        }
    }

    void AnimateEntry()
    {
        
    }

    void StopAnimatingCard(CardUI u)
    {

    }

    void AnimateInventoryMouseOver(BaseTower t, CardUI u)
    {
        anims.StopAnimating(u);
        anims.StartAnimating(u, MoveCard(u, new Vector2(100, 100), Vector2.zero));
    }

    void AnimateInventoryMouseExit(BaseTower t, CardUI u)
    {

    }

    IEnumerator MoveCard(CardUI u, Vector2 start, Vector2 end)
    {
        float time = 0f;
        float animTime = 1f;
        RectTransform uT = u.GetComponent<RectTransform>();
        while(time < animTime)
        {
            float progress = 1 - ((animTime - time) / animTime);
            progress = movementCurve.Evaluate(progress);
            uT.anchoredPosition = Vector2.Lerp(start, end, progress);

            time += Time.deltaTime;
            yield return null;   
        }
        uT.anchoredPosition = end;
        anims.StopAnimating(u);
    }

    private class UIAnimations : MonoBehaviour
    {
        private List<CardUI> cards = new List<CardUI>();
        private List<Coroutine> anims = new List<Coroutine>();

        public bool IsAnimating(CardUI card) => cards.Contains(card);

        public void StopAnimating(CardUI card)
        {
            if(IsAnimating(card))
            {
                int index = cards.IndexOf(card);
                Coroutine c = anims[index];
                StopCoroutine(c);
                cards.Remove(card);
            }
        }
        public void StartAnimating(CardUI card, IEnumerator action)
        {
            if (!IsAnimating(card))
            {
                cards.Add(card);
                anims.Add(StartCoroutine(action));
            }
        }
    }
}
