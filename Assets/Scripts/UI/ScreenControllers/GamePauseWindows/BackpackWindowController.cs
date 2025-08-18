using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KidGame.Core;
using KidGame.UI;
using KidGame.UI.Game;
using TMPro;
using UnityEngine;
using Utils;

public class RefreshBackpackSignal : ASignal
{
    
}
public class CloseBackpackWindowSignal : ASignal
{
    
}

[Serializable]
public class BackpackProp : WindowProperties
{
    //移动物品时的对面的物品列表引用
    private List<ISlotInfo> targetList;

    public BackpackProp(List<ISlotInfo> targetList)
    {
        hideOnForegroundLost = false;
        this.targetList = targetList;
    }
}
public class BackpackWindowController : WindowController<BackpackProp>
{
    private UICircularScrollView scrollView;
    private UICircularScrollView pocketScrollView;


    private List<MaterialSlotInfo> _materialSlotInfos;
    private List<TrapSlotInfo> _trapSlotInfos;

    private List<ISlotInfo> _tempSlotInfos;
    //private List<ISlotInfo> _trapSlotInfos;
    private GameObject detailPanel;
    private TextMeshProUGUI detailText;

    // 陷阱列表最大容量 tips：移动到GlobalValue脚本里了，别的地方也要用
    //private const int MAX_BACKPACK_TRAP = 10; // 背包陷阱最大数量
    //private const int MAX_POCKET_TRAP = 5; // 口袋最大数量

    protected override void Awake()
    {
        base.Awake();
        detailPanel = transform.Find("DetailPanel").gameObject;
        detailText = transform.Find("DetailPanel/DetailText").GetComponent<TextMeshProUGUI>();
        detailPanel.SetActive(false);
        
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        Signals.Get<ShowItemDetailSignal>().AddListener(OnShowItemDetail);
        Signals.Get<HideItemDetailSignal>().AddListener(OnHideItemDetail);
        Signals.Get<RefreshBackpackSignal>().AddListener(RefreshLists);
        Signals.Get<CloseBackpackWindowSignal>().AddListener(AddCloseAction);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        Signals.Get<ShowItemDetailSignal>().RemoveListener(OnShowItemDetail);
        Signals.Get<HideItemDetailSignal>().RemoveListener(OnHideItemDetail);
        Signals.Get<RefreshBackpackSignal>().RemoveListener(RefreshLists);
        Signals.Get<CloseBackpackWindowSignal>().RemoveListener(AddCloseAction);
    }

    public override void UI_Close()
    {
        base.UI_Close();
        GameManager.Instance.GameResume();
    }

    private void AddCloseAction()
    {
        PlayerController.Instance.InputSettings.OnInteractionPressWithoutTime += UI_Close;
    }
    protected override void OnPropertiesSet()
    {
        GameManager.Instance.GamePause();
        scrollView = transform.Find("BackPag/ScrollView").GetComponent<UICircularScrollView>();
        pocketScrollView = transform.Find("PlayerPocket/ScrollView").GetComponent<UICircularScrollView>();

        _tempSlotInfos = PlayerBag.Instance.GetQuickAccessBag();
        _materialSlotInfos = PlayerBag.Instance.BackBag.Where(x => x.ItemData.UseItemType == KidGame.UseItemType.Material)
            .Cast<MaterialSlotInfo>().ToList();
        _trapSlotInfos = PlayerBag.Instance.BackBag.Where(x => x.ItemData.UseItemType == KidGame.UseItemType.trap)
            .Cast<TrapSlotInfo>().ToList();

        scrollView.Init(_materialSlotInfos.Count + _trapSlotInfos.Count, OnBagCellUpdate, OnBagCellClick, null);
        pocketScrollView.Init(_tempSlotInfos.Count, OnPocketCellUpdate, OnPocketCellClick, null);
    }
    
    
    protected override void WhileHiding()
    {
        OnHideItemDetail();
        PlayerController.Instance.InputSettings.OnInteractionPressWithoutTime -= UI_Close;
    }

    private void OnBagCellClick(GameObject cell, int index)
    {
        int cellIndex = index - 1;
        // 原本的道具栏与背包互换代码移动到了playerBag.cs
        PlayerBag.Instance.MoveItemToQuickAccessBag(cellIndex);
        RefreshLists();
        OnHideItemDetail();
    }

    private void OnPocketCellClick(GameObject cell, int index)
    {
        int cellIndex = index - 1;
        // 原本的道具栏与背包互换代码移动到了playerBag.cs
        PlayerBag.Instance.MoveItemToBackBag(cellIndex);
        RefreshLists();
        OnHideItemDetail();
    }
    
    private void RefreshLists()
    {
        _tempSlotInfos = PlayerBag.Instance.GetQuickAccessBag();
        _materialSlotInfos = PlayerBag.Instance.BackBag.Where(x => x.ItemData.UseItemType == KidGame.UseItemType.Material)
            .Cast<MaterialSlotInfo>().ToList();
        _trapSlotInfos = PlayerBag.Instance.BackBag.Where(x => x.ItemData.UseItemType == KidGame.UseItemType.trap)
            .Cast<TrapSlotInfo>().ToList();

        scrollView.ShowList(_materialSlotInfos.Count + _trapSlotInfos.Count);
        pocketScrollView.ShowList(_tempSlotInfos.Count);
    }


    private void OnBagCellUpdate(GameObject cell, int index)
    {
        CellUI cellUI = cell.GetComponent<CellUI>();
        int realIndex = index - 1;

        if (realIndex < _materialSlotInfos.Count)
        {
            cellUI.SetUIWithMaterial(_materialSlotInfos[realIndex]);
        }
        else if (realIndex - _materialSlotInfos.Count < _trapSlotInfos.Count)
        {
            ISlotInfo slot = _trapSlotInfos[realIndex - _materialSlotInfos.Count];
            cellUI.SetUIWithGenericSlot(slot);
        }
    }

    private void OnPocketCellUpdate(GameObject cell, int index)
    {
        CellUI cellUI = cell.GetComponent<CellUI>();
        ISlotInfo slot = _tempSlotInfos[index - 1];
        cellUI.SetUIWithGenericSlot(slot);
    }

    private void OnShowItemDetail(CellUI cellUI)
    {
        detailPanel.transform.position = cellUI.detailPoint.position;
        detailText.text = cellUI.detailText;
        detailPanel.SetActive(true);
    }

    private void OnHideItemDetail()
    {
        detailPanel.SetActive(false);
    }
}