using KidGame;
using KidGame.Core;
using System.Collections;
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
    public Transform detailPoint;

    private float timer = 0f;
    private const float delay = 0.6f; // 悬停时间阈值

    private void Awake()
    {
        eventTrigger = GetComponent<EventTrigger>();

        AddEventTrigger();
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
            timer += Time.unscaledTime;
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

    public void SetUIWithGenericSlot(ISlotInfo slot)
    {
        switch (slot.ItemData.UseItemType)
        {
            case UseItemType.trap:
                SetUIWithTrap(slot as TrapSlotInfo);
                break;
            case UseItemType.weapon:
                
                break;
            case UseItemType.food:
                
                break;
            case UseItemType.Material:
                SetUIWithMaterial(slot as MaterialSlotInfo);
                break;
            default:
                break;
        }
    }

    public void SetUIWithMaterial(MaterialSlotInfo material)
    {
        icon.sprite = Resources.Load<Sprite>(material.materialData.materialIconPath);
        count.text = material.amount.ToString();
        detailText = material.materialData.materialDesc;
    }

    public void SetUIWithTrap(TrapSlotInfo trap)
    {
        icon.sprite = Resources.Load<Sprite>(trap.trapData.trapIconPath);
        count.text = trap.amount.ToString();
        detailText = trap.trapData.trapDesc;
    }
}