using KidGame;
using KidGame.Core;
using System.Collections;
using KidGame.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;




public class CellUI : MonoBehaviour
{
    private EventTrigger eventTrigger;

    public Image icon;
    public TextMeshProUGUI count;
    public string detailText;
    public Transform detailPoint;
    public Transform moveItemPanelPoint;

    private float timer = 0f;
    private const float delay = 0.4f; // ��ͣʱ����ֵ

    private void Awake()
    {
        eventTrigger = GetComponent<EventTrigger>();

        AddEventTrigger();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        timer = 0f; // ���ü�ʱ��
        StartCoroutine(ShowDetailPanel()); // ��ʼ��ʱ
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines(); // ֹͣ����Э��
        timer = 0f; // ���ü�ʱ��
        
        UIHelper.Instance.HideItemDetail();
        //Signals.Get<HideItemDetailSignal>().Dispatch();
    }

    private IEnumerator ShowDetailPanel() {
        timer = 0f;  // ȷ����ʱ����0��ʼ
        while (timer < delay) {
            timer += Time.unscaledDeltaTime;  // ʹ�÷���������ʱ��
            yield return null;  // ��ʹtimeScale=0���Ի�ȴ���һ֡
        }
        // ��ʱ��������ʾ�������
        
        UIHelper.Instance.ShowItemDetail(this);
        //Signals.Get<ShowItemDetailSignal>().Dispatch(this);
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