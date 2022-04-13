using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject endBuildButton;
    [SerializeField] Transform inventoryArea;
    [SerializeField] RectTransform selectedArea;
    [SerializeField] AnimationCurve movementCurve;
    [SerializeField] RectTransform difficultyScrollContent;
    [SerializeField] IndicatorAnimation indicatorAnimation;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] DifficultyNotificationAnimation difficultyNotifAnim;
    [SerializeField] GameObject passageErrorText;
    [SerializeField] GameObject rangeIndicator;
    [SerializeField] TextMeshProUGUI playerHealthText;
    [SerializeField] TextMeshProUGUI playerScoreValText;
    [SerializeField] GameObject gameRestartButton;
    [SerializeField] FusionUIManager fusionUI;
    private const float difficultyContainerLength = 100;

    private List<CardUI> cards = new List<CardUI>();

    private UIAnimations anims;

    private float oddYOffset = -30f;
    private float xOffset = 90;
    private float yOffsetMouseOver = 190f;

    private CardUI selectedCard = null; // card in inventory selected
    private CardUI worldCard = null; // tower in the world selected by player

    private int mouseTracker = 0;
    private int entryTracker = 0;

    private void Awake()
    {
        anims = gameObject.AddComponent<UIAnimations>();
        instance = this;
    }

    public void ToggleFusionUI() { fusionUI.Toggle(); }
    public void ShowFusionUI() { fusionUI.Show(); }
    public void HideFusionUI() { fusionUI.Hide(); }

    public void AddFusionCard(BaseTower t)
    {
        fusionUI.AddTower(t);
    }

    public GameObject GetCardUIPrefab() => cardPrefab;

    public void SetPlayerHealth(int num)
    {
        if (num < 0)
            num = 0;

        playerHealthText.text = num.ToString();

        if (num <= 0)
        {
            StartCoroutine(SceneSwitchEnd());
            //gameRestartButton.SetActive(true);

        }
    }

    public void SetPlayerScore()
    {
        playerScoreValText.text = ScoreTracker.inst.getScore().ToString();
    }

    

    public void ShowRangeIndicator(Vector3 pos, float radius)
    {
        rangeIndicator.SetActive(true);
        rangeIndicator.transform.position = pos + new Vector3(0, 0.5f, 0);
        rangeIndicator.transform.localScale = new Vector3(radius, 1, radius);
    }

    public void HideRangeIndicator()
    {
        rangeIndicator.SetActive(false);
    }

    public void BeginBuild()
    {
        endBuildButton.SetActive(true);
        infoText.gameObject.SetActive(true);
    }

    public void UpdateBuildTime(float time)
    {
        time = Mathf.RoundToInt(time);
        infoText.text = "Time Left: <size=24><b>" + time + "</b></size> seconds.";
    }

    public void EndBuild()
    {
        WaveManager.instance.building = false;
        endBuildButton.SetActive(false);
        infoText.text = "";
        HideRangeIndicator();
    }

    public void SetPassageErrorText(bool set)
    {
        passageErrorText.SetActive(set);
    }

    public void SetWaveText(int enemiesLeft)
    {
        infoText.text = "Enemies Left: <size=24><b>" + enemiesLeft + "</size></b>";
    }

    public void SetSpawnIndicator(int dir, Vector3 worldPos)
    {
        indicatorAnimation.SetIndicator(dir, worldPos);
    }

    public void ClearSpawnIndicators()
    {
        indicatorAnimation.StopIndicators();
    }

    public void UpdateDifficultySlider(float difficulty)
    {
        float scrollX = -difficulty * difficultyContainerLength;
        difficultyScrollContent.anchoredPosition = new Vector3(scrollX, 0, 0);
    }

    public void NotifyDifficultyChange(string name, Color c)
    {
        difficultyNotifAnim.Animate(name, c);
    }

    public void DestroyWorldCard()
    {
        if (worldCard != null)
        {
            Destroy(worldCard.gameObject);
            worldCard = null;
        }
    }

    public void CreateWorldCard(BaseTower t)
    {
        DestroyWorldCard();

        GameObject prefab = Instantiate(cardPrefab, inventoryArea);
        CardUI u = prefab.GetComponent<CardUI>();
        u.SetCard(t);
        u.FollowWorldPoint();

        if(WaveManager.instance.building)
            u.ShowOptions(0);

        worldCard = u;
    }

    public void CreateCard(BaseTower t)
    {
        CreateCard(t, false);
    }

    public void CreateCard(BaseTower t, bool skipEntry)
    {
        GameObject prefab = Instantiate(cardPrefab, inventoryArea);
        CardUI u = prefab.GetComponent<CardUI>();
        u.SetCard(t);
        AddCard(u, skipEntry);
    }

    void AddCard(CardUI card)
    {
        AddCard(card, false);
    }

    void AddCard(CardUI card, bool skipEntry)
    {
        cards.Add(card);
        card.transform.SetParent(inventoryArea);

        card.AddEvent(AnimateInventoryMouseOver, EventTriggerType.PointerEnter);
        card.AddEvent(AnimateInventoryMouseExit, EventTriggerType.PointerExit);
        card.AddEvent(SelectCard, EventTriggerType.PointerClick);
        ArrangeCards();
        if(!skipEntry)
            AnimateEntry(card);
    }

    public void DestroySelectedCard()
    {
        if (selectedCard != null)
            DestroyCard(selectedCard);
    }

    void DestroyCard(CardUI card)
    {
        if(cards.Contains(card))
        {
            anims.StopAnimating(card);
            cards.Remove(card);
            Destroy(card.gameObject);
            ArrangeCards();
        }
    }

    public void DestroyCardIfExists(BaseTower t)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].GetTowerRef() == t)
            {
                DestroyCard(cards[i]);
                break;
            }
        }
    }

    Vector2 GetCardInvPos(CardUI card)
    {
        int index = cards.IndexOf(card);
        return GetCardInvPos(index);
    }

    Vector2 GetCardInvPos(int i)
    {
        bool odd = i % 2 == 1;
        float yOff = odd ? oddYOffset : 0;
        float xOff = xOffset * i;

        return new Vector2(xOff, yOff);
    }

    void ArrangeCards()
    {
        for(int i = 0; i < cards.Count; i++)
        {
            if (!cards[i].entering && selectedCard != cards[i])
            {
                bool odd = i % 2 == 1;
                float yOff = odd ? oddYOffset : 0;
                float xOff = xOffset * i;
                int zIndex = odd ? 1 : 0;

                RectTransform cardRT = cards[i].GetComponent<RectTransform>();
                cardRT.SetSiblingIndex(zIndex);

                AnimateToInventory(cards[i]);
            }
        }
    }

    public void UnselectCard()
    {
        if(selectedCard != null)
        {
            selectedCard.HideOptions();
            AnimateToInventory(selectedCard);
            selectedCard = null;
        }
    }

    public void SelectCard(int index)
    {
        if (index < 0 || index >= cards.Count || !WaveManager.instance.building)
            return;

        BaseTower tower = cards[index].GetTowerRef();
        CardUI cardUI = cards[index];
        SelectCard(tower, cardUI);
    }

    void SelectCard(BaseTower t, CardUI u)
    {
        if (!WaveManager.instance.building)
            return;

        // Unselect card if not same
        if (selectedCard != u && selectedCard != null)
            UnselectCard();

        if (selectedCard != u && !u.entering)
        {
            selectedCard = u;
            u.ShowOptions(1);
            InputHandler.instance.UI_RequestTower(t);

            AnimateSelect(u);
        }
    }
    
    void StopAnimating(CardUI u)
    {
        anims.StopAnimating(u);
    }

    void AnimateSelect(CardUI u)
    {
        Vector2 cardPos = u.GetComponent<RectTransform>().anchoredPosition;

        Vector2 selectPos = selectedArea.position;

        float animTime = 1f;

        anims.StopAnimating(u);
        anims.StartAnimating(u, MoveCard(u, cardPos, selectPos, animTime));
        
    }

    void AnimateEntry(CardUI u)
    {
        RectTransform cRT = u.GetComponent<RectTransform>();
        Vector2 cardPos = cRT.anchoredPosition;
        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f) - new Vector2(cRT.sizeDelta.x / 2f, -cRT.sizeDelta.y / 2f);

        float spacing = cRT.sizeDelta.x + 20;

        float cardX = (entryTracker % 2 == 0 ? -1 : 1) * Mathf.Round((float)entryTracker / 2f + 0.49f) * spacing;
        //bunch of magical math stuff that places cards like the following: [ ... , 5, 3, 1, 2, 4, ... ]
        center += new Vector2(cardX, 0);

        float animTime = 1f;
        float centerWaitTime = 1.5f;

        anims.StopAnimating(u);
        //anims.StartAnimating(u, AnimateWaitEntry(u, cardPos, center, invPos, animTime, centerWaitTime));
        StartCoroutine(AnimateWaitEntry(u, cardPos, center, animTime, centerWaitTime));
    }

    void AnimateToInventory(CardUI u)
    {
        Vector2 cardPos = u.GetComponent<RectTransform>().anchoredPosition;

        Vector2 invPos = GetCardInvPos(u);

        float animTime = 1f;

        anims.StopAnimating(u);
        anims.StartAnimating(u, MoveCard(u, cardPos, invPos, animTime));
    }

    void AnimateInventoryMouseOver(BaseTower t, CardUI u)
    {
        mouseTracker += 1; // used for tracking selections, so we know if we're clicking on a card
        if (u != selectedCard && !u.entering)
        {
            Vector2 cardPos = u.GetComponent<RectTransform>().anchoredPosition;

            Vector2 invPos = GetCardInvPos(u);
            Vector2 invPosAbove = invPos + new Vector2(0, yOffsetMouseOver);

            float animTime = 0.2f + 0.8f * Mathf.Abs((cardPos.y - invPosAbove.y)) / yOffsetMouseOver; // time to animate is proportional to distance from goal

            anims.StopAnimating(u);
            anims.StartAnimating(u, MoveCard(u, cardPos, invPosAbove, animTime));
        }
    }

    void AnimateInventoryMouseExit(BaseTower t, CardUI u)
    {
        mouseTracker -= 1;
        if (u != selectedCard && !u.entering)
        {
            Vector2 cardPos = u.GetComponent<RectTransform>().anchoredPosition;

            Vector2 invPos = GetCardInvPos(u);

            float animTime = 0.2f + 0.8f * Mathf.Abs((cardPos.y - invPos.y)) / yOffsetMouseOver;

            anims.StopAnimating(u);
            anims.StartAnimating(u, MoveCard(u, cardPos, invPos, animTime));
        }
    }

    IEnumerator AnimateWaitEntry(CardUI u, Vector2 start, Vector2 middle, float animTime, float waitTime)
    {
        u.entering = true;
        int youngest = entryTracker;
        entryTracker += 1;
        StartCoroutine(MoveCard(u, start, middle, animTime));
        yield return new WaitForSeconds(animTime + waitTime);
        Vector2 invPos = GetCardInvPos(u);
        StartCoroutine(MoveCard(u, middle, invPos, animTime));
        yield return new WaitForSeconds(animTime);
        entryTracker = youngest < entryTracker ? youngest : entryTracker;
        //entryTracker = entryTracker < 0 ? 0 : entryTracker;
        u.entering = false;
        ArrangeCards();
    }

    IEnumerator MoveCard(CardUI u, Vector2 start, Vector2 end, float animTime)
    {
        float time = 0f;

        RectTransform uT = u.GetComponent<RectTransform>();
        while(time < animTime && u != null)
        {
            float progress = 1 - ((animTime - time) / animTime);
            progress = movementCurve.Evaluate(progress);
            uT.anchoredPosition = Vector2.Lerp(start, end, progress);

            time += Time.deltaTime;
            yield return null;   
        }

        if(u != null)
            uT.anchoredPosition = end;

        anims.StopAnimating(u);
    }

    public void Button_Restart() { SceneManager.LoadScene("MenuScene"); }

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
                cards.RemoveAt(index);
                anims.RemoveAt(index);
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

    IEnumerator SceneSwitchEnd()
    {
        SceneManager.LoadScene("EndScene", LoadSceneMode.Single);
        yield return null;
        SceneManager.UnloadSceneAsync("MainScene");
    }

}
