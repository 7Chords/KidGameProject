using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI;
using KidGame.UI.Game;
using TMPro;
using UnityEngine;
using Utils;

public class BackpackWindowController : WindowController
{
    private UICircularScrollView scrollView;
    private UICircularScrollView pocketScrollView;
    private List<MaterialSlotInfo> _materialSlotInfos;
    private List<TrapSlotInfo> _trapSlotInfos;
    private List<TrapSlotInfo> _tempTrapSlotInfos;
    private GameObject detailPanel;
    private TextMeshProUGUI detailText;
    
    // 陷阱列表最大容量（根据实际需求调整）
    private const int MAX_BACKPACK_TRAP = 10;  // 背包陷阱最大数量
    private const int MAX_POCKET_TRAP = 5;     // 口袋最大数量

    protected override void Awake()
    {
        base.Awake();
        detailPanel = transform.Find("DetailPanel").gameObject;
        detailText = transform.Find("DetailPanel/DetailText").GetComponent<TextMeshProUGUI>();
        detailPanel.SetActive(false);
        Signals.Get<ShowItemDetailSignal>().AddListener(OnShowItemDetail);
        Signals.Get<HideItemDetailSignal>().AddListener(OnHideItemDetail);
      
    }

    protected override void OnPropertiesSet()
    {
        scrollView = transform.Find("BackPag/ScrollView").GetComponent<UICircularScrollView>();
        pocketScrollView = transform.Find("PlayerPocket/ScrollView").GetComponent<UICircularScrollView>();
        _materialSlotInfos = PlayerBag.Instance.GetMaterialSlots();
        _trapSlotInfos = PlayerBag.Instance.GetTrapSlots();
        _tempTrapSlotInfos = PlayerBag.Instance.GetTempTrapSlots();
        scrollView.Init(_materialSlotInfos.Count + _trapSlotInfos.Count, OnBagCellUpdate,OnBagCellClick, null);
        pocketScrollView.Init(_tempTrapSlotInfos.Count, OnPocketCellUpdate,OnPocketCellClick, null);
    }

   
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Signals.Get<ShowItemDetailSignal>().RemoveListener(OnShowItemDetail);
        Signals.Get<HideItemDetailSignal>().RemoveListener(OnHideItemDetail);
        
    }
    
    
    protected override void WhileHiding()
    {
        OnHideItemDetail();
    }

    // 背包格子点击处理
    private void OnBagCellClick(GameObject cell, int index)
    {
        int cellIndex = index - 1; // 转换为0-based索引
        int materialCount = _materialSlotInfos.Count;
        
        // 仅处理陷阱格子（材料格子索引 < materialCount）
        if (cellIndex >= materialCount)
        {
            int trapIndex = cellIndex - materialCount;
            if (trapIndex >= 0 && trapIndex < _trapSlotInfos.Count)
            {
                MoveTrapToPocket(trapIndex);
            }
        }
    }

    // 口袋格子点击处理
    private void OnPocketCellClick(GameObject cell, int index)
    {
        int cellIndex = index - 1; // 转换为0-based索引
        if (cellIndex >= 0 && cellIndex < _tempTrapSlotInfos.Count)
        {
            MoveTrapToBackpack(cellIndex);
        }
    }
    // 移动陷阱到口袋（支持已满时交换）
    private void MoveTrapToPocket(int trapIndex)
    {
        // 获取源陷阱数据（背包中的陷阱）
        TrapSlotInfo sourceTrap = _trapSlotInfos[trapIndex];
        TrapSlotInfo replacedTrap = null;

        // 检查口袋是否已满
        if (_tempTrapSlotInfos.Count < MAX_POCKET_TRAP)
        {
            // 口袋未满：直接添加源陷阱
            _tempTrapSlotInfos.Add(sourceTrap);
        }
        else
        {
            // 口袋已满：保存被替换的陷阱，并替换为源陷阱
            replacedTrap = _tempTrapSlotInfos[MAX_POCKET_TRAP - 1]; // 保存最后一个陷阱
            _tempTrapSlotInfos[MAX_POCKET_TRAP - 1] = sourceTrap;   // 替换最后一个陷阱
        }

        // 从背包移除源陷阱
        _trapSlotInfos.RemoveAt(trapIndex);

        // 如果有被替换的陷阱，添加到背包原位置
        if (replacedTrap != null)
        {
            _trapSlotInfos.Insert(trapIndex, replacedTrap); // 插入到原陷阱位置（保持列表长度不变）
        }

        // 刷新UI
        RefreshLists();
    }

// 移动陷阱到背包（支持已满时交换）
    private void MoveTrapToBackpack(int trapIndex)
    {
        // 获取源陷阱数据（口袋中的陷阱）
        TrapSlotInfo sourceTrap = _tempTrapSlotInfos[trapIndex];
        TrapSlotInfo replacedTrap = null;

        // 检查背包陷阱是否已满
        if (_trapSlotInfos.Count < MAX_BACKPACK_TRAP)
        {
            // 背包未满：直接添加源陷阱
            _trapSlotInfos.Add(sourceTrap);
        }
        else
        {
            // 背包已满：保存被替换的陷阱，并替换为源陷阱
            replacedTrap = _trapSlotInfos[MAX_BACKPACK_TRAP - 1]; // 保存最后一个陷阱
            _trapSlotInfos[MAX_BACKPACK_TRAP - 1] = sourceTrap;   // 替换最后一个陷阱
        }

        // 从口袋移除源陷阱
        _tempTrapSlotInfos.RemoveAt(trapIndex);

        // 如果有被替换的陷阱，添加到口袋原位置
        if (replacedTrap != null)
        {
            _tempTrapSlotInfos.Insert(trapIndex, replacedTrap); // 插入到原陷阱位置（保持列表长度不变）
        }

        // 刷新UI
        RefreshLists();
    }

    // 刷新两个列表
    private void RefreshLists()
    {
        // 刷新背包列表（材料+陷阱总数变化）
        scrollView.ShowList(_materialSlotInfos.Count + _trapSlotInfos.Count);
        // 刷新口袋列表
        pocketScrollView.ShowList(_tempTrapSlotInfos.Count);
        
        PlayerBag.Instance.TrapBagUpdated();
    }

    private void OnBagCellUpdate(GameObject cell, int index)
    {
        CellUI cellUI = cell.GetComponent<CellUI>();
        if (index - 1 < _materialSlotInfos.Count)
        {
            cellUI.SetUIWithMaterial(_materialSlotInfos[index - 1]);
        }
        else if (index - 1 - _materialSlotInfos.Count < _trapSlotInfos.Count)
        {
            cellUI.SetUIWithTrap(_trapSlotInfos[index - 1 - _materialSlotInfos.Count]);
        }
    }

    private void OnPocketCellUpdate(GameObject cell, int index)
    {
        CellUI cellUI = cell.GetComponent<CellUI>();
        cellUI.SetUIWithTrap(_tempTrapSlotInfos[index-1]);
    }
    private void OnShowItemDetail(CellUI cellUI)
    {
        /*// 获取鼠标在屏幕上的位置
        Vector2 mousePosition = Input.mousePosition;
        // 将鼠标屏幕坐标转换为 Canvas 的局部坐标
        RectTransform canvasRectTransform = UIController.Instance.uiCanvas.transform as RectTransform;
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePosition, null, out localMousePosition);
        // 应用偏移量（往右下一点）
        Vector2 offset = new Vector2(10, -10); // 偏移量，可以根据需要调整
        localMousePosition += offset;

        // 设置详细描述面板的位置
        detailPanel.GetComponent<RectTransform>().anchoredPosition = localMousePosition;*/

        detailPanel.transform.position = cellUI.detailPoint.position;
        detailText.text = cellUI.detailText;
        detailPanel.SetActive(true);
    }

    private void OnHideItemDetail()
    {
        detailPanel.SetActive(false);
    }
}