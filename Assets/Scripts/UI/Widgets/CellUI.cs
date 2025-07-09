using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI.Game;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;


public class ShowItemDetailSignal : ASignal<CellUI>
{
}

public class HideItemDetailSignal : ASignal
{
}

public class CellUI : MonoBehaviour
{
    private EventTrigger eventTrigger;

    public Image icon;
    public TextMeshProUGUI count;
    public string detailText;

    private float timer = 0f;
    private const float delay = 0.6f; // 悬停时间阈值

    private void Awake()
    {
        eventTrigger = GetComponent<EventTrigger>();
        
        AddEventTrigger();
    }

    public void SetUIWithMaterial(MaterialSlotInfo material)
    {
        icon.sprite = material.materialData.materialIcon;
        count.text = material.amount.ToString();
        detailText = material.materialData.materialDesc;
    }

    public void SetUIWithTrap(TrapSlotInfo trap)
    {
        icon.sprite = trap.trapData.trapIcon;
        count.text = trap.amount.ToString();
        detailText = trap.trapData.trapDesc;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        timer = 0f; // 重置计时器
        StartCoroutine(ShowDetailPanel()); // 开始计时
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines(); // 停止所有协程
        timer = 0f; // 重置计时器
        Signals.Get<HideItemDetailSignal>().Dispatch();
    }

    private IEnumerator ShowDetailPanel()
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                Signals.Get<ShowItemDetailSignal>().Dispatch(this);
                break;
            }

            yield return null;
        }
    }

    private void AddEventTrigger()
    {
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
        
        EventTrigger.Entry entryEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entryEnter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        trigger.triggers.Add(entryEnter);
        
        EventTrigger.Entry entryExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        entryExit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
        trigger.triggers.Add(entryExit);
    }
}