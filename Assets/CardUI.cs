using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

// all callback events require BaseTower and CardUI as args
[RequireComponent(typeof(EventTrigger))]
public class CardUI : MonoBehaviour
{
    [SerializeField] Text title;
    [SerializeField] Text stats;
    [SerializeField] Image icon;
    [SerializeField] Transform starsArea;
    [SerializeField] GameObject starPrefab;
    private BaseTower towerRef;

    [SerializeField] EventTrigger trigger;

    private void Start()
    {
        AddEvent(testcallback, EventTriggerType.PointerEnter);
    }

    private void testcallback(BaseTower t, CardUI u)
    {
        print("Called!");
    }

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
    

    public void AddEvent(Action<BaseTower, CardUI> callback, EventTriggerType triggerType)
    {
        EventTrigger.Entry newEvent = new EventTrigger.Entry();
        newEvent.eventID = triggerType;
        newEvent.callback.AddListener((data) => { callback(towerRef, this); });
        trigger.triggers.Add(newEvent);
    }

    private void SetTitle(string txt) { title.text = txt; }
    private void SetStats(string txt) { stats.text = txt; }
    private void SetIcon(Sprite img) { icon.sprite = img; }

    private void SetQuality(int num)
    {
        if (starsArea.childCount != 0)
            foreach (Transform c in starsArea)
                Destroy(c.gameObject);
        for (int i = 0; i < num; i++)
            Instantiate(starPrefab, starsArea);
    }
}
