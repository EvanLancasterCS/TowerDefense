using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FusionUIManager : MonoBehaviour
{
    GameObject cardPrefab;
    CardUI card1 = null, card2 = null;
    [SerializeField] Transform card1Area, card2Area;
    [SerializeField] TextMeshProUGUI textResults;
    CanvasGroup canvasGroup;
    RectTransform rect;
    void Start()
    {
        cardPrefab = UIManager.instance.GetCardUIPrefab();
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
    }

    public void AddTower(BaseTower t)
    {
        if (card1 == null)
        {
            GameObject newCard = Instantiate(cardPrefab);
            CardUI card = newCard.GetComponent<CardUI>();
            card.SetCard(t);

            newCard.transform.SetParent(card1Area);
            newCard.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            card1 = card;
        }
        else if (t != card1.GetTowerRef())
        {
            GameObject newCard = Instantiate(cardPrefab);
            CardUI card = newCard.GetComponent<CardUI>();
            card.SetCard(t);

            ClearSlot(1);
            newCard.transform.SetParent(card2Area);
            newCard.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            card2 = card;
        }
    }

    public void Button_AttemptFusion()
    {
        if (card1 != null && card2 != null)
        {
            BaseTower t1 = card1.GetTowerRef();
            BaseTower t2 = card2.GetTowerRef();
            if (t1.GetID() == t2.GetID())
            {
                if (t1.HasUpgrade())
                {
                    int t1ID = TowerManager.instance.GetUnplacedTowerID(t1);
                    int t2ID = TowerManager.instance.GetUnplacedTowerID(t2);
                    int newTowerID = TowerManager.instance.CombineTowers(t1ID, t2ID);
                    BaseTower newTower = TowerManager.instance.GetUnplacedTower(newTowerID);
                    UIManager.instance.CreateCard(newTower);
                    InputHandler.instance.ClearAll();
                    Hide();
                    //DisplayResult("<color=green>Success!</color>");
                }
                else
                {
                    DisplayResult("<color=red>Failure. This card is not upgradable.<color=red>");
                }
            }
            else
            {
                DisplayResult("<color=red>Failure. Ensure both cards are the same.<color=red>");
            }
        }
        else
        {
            DisplayResult("<color=red>Failure. Two cards are required.<color=red>");
        }
    }

    public void DisplayResult(string results)
    {
        textResults.gameObject.SetActive(true);
        textResults.text = results;
    }

    public void HideResults()
    {
        textResults.gameObject.SetActive(false);
    }

    public void ClearSlot(int i)
    {
        if(i == 0 && card1 != null)
        {
            Destroy(card1.gameObject);
            card1 = null;
        }
        else if(i == 1 && card2 != null)
        {
            Destroy(card2.gameObject);
            card2 = null;
        }
    }

    public void ClearSlots()
    {
        ClearSlot(0);
        ClearSlot(1);
    }

    public void Show()
    {
        if (canvasGroup.alpha == 0)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            rect.anchoredPosition = Vector2.zero;
        }
    }

    public void Hide()
    {
        ClearSlots();
        HideResults();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Toggle()
    {
        if (canvasGroup.alpha == 0)
            Show();
        else
            Hide();
    }
}
