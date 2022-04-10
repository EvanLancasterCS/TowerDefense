using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;

// all callback events require BaseTower and CardUI as args
[RequireComponent(typeof(EventTrigger))]
public class CardUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI stats;
    [SerializeField] Image icon;
    [SerializeField] Transform starsArea;
    [SerializeField] GameObject starPrefab;
    [SerializeField] GameObject optionsArea, retrieveText, fusionText;
    private BaseTower towerRef;
    public bool entering = false;

    [SerializeField] EventTrigger trigger;

    public void SetCard(BaseTower _towerRef)
    {
        towerRef = _towerRef;
        string title = towerRef.GetName();
        string stats = towerRef.GetStats();
        int quality = towerRef.GetQuality();
        Sprite icon = null; // work in prog

        SetTitle(title);
        SetStats(stats);
        SetIcon(icon);
        SetQuality(quality);
    }

    public void ShowOptions(int type)
    {
        if (type == 0 && towerRef.IsRetrievable())
        {
            optionsArea.SetActive(true);
            retrieveText.SetActive(true);
            fusionText.SetActive(false);
        }
        else if (type == 1 && towerRef.HasUpgrade())
        {
            optionsArea.SetActive(true);
            retrieveText.SetActive(false);
            fusionText.SetActive(true);
        }
    }

    public void HideOptions()
    {
        optionsArea.SetActive(false);
    }
    
    public void FollowWorldPoint()
    {
        StartCoroutine(Follow(towerRef.transform.position + new Vector3(0, 2, 0)));
    }

    private IEnumerator Follow(Vector3 pos)
    {
        while(true)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(pos);
            transform.position = screenPos;
            yield return null;
        }
    }

    public void AddEvent(Action<BaseTower, CardUI> callback, EventTriggerType triggerType)
    {
        EventTrigger.Entry newEvent = new EventTrigger.Entry();
        newEvent.eventID = triggerType;
        newEvent.callback.AddListener((data) => { callback(towerRef, this); });
        trigger.triggers.Add(newEvent);
    }

    private void SetTitle(string txt) { title.text = txt; }
    private void SetStats(string txt) { stats.text = txt; }
    private void SetIcon(Sprite img) { }//i//con.sprite = img; }

    private void SetQuality(int num)
    {
        if (starsArea.childCount != 0)
            foreach (Transform c in starsArea)
                Destroy(c.gameObject);
        for (int i = 0; i < num; i++)
            Instantiate(starPrefab, starsArea);
    }

    public BaseTower GetTowerRef() => towerRef;
}
