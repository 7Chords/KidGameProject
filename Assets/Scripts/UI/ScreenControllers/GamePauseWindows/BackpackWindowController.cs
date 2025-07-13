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
    
    // �����б��������������ʵ�����������
    private const int MAX_BACKPACK_TRAP = 10;  // ���������������
    private const int MAX_POCKET_TRAP = 5;     // �ڴ��������

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

    // �������ӵ������
    private void OnBagCellClick(GameObject cell, int index)
    {
        int cellIndex = index - 1; // ת��Ϊ0-based����
        int materialCount = _materialSlotInfos.Count;
        
        // ������������ӣ����ϸ������� < materialCount��
        if (cellIndex >= materialCount)
        {
            int trapIndex = cellIndex - materialCount;
            if (trapIndex >= 0 && trapIndex < _trapSlotInfos.Count)
            {
                MoveTrapToPocket(trapIndex);
            }
        }
    }

    // �ڴ����ӵ������
    private void OnPocketCellClick(GameObject cell, int index)
    {
        int cellIndex = index - 1; // ת��Ϊ0-based����
        if (cellIndex >= 0 && cellIndex < _tempTrapSlotInfos.Count)
        {
            MoveTrapToBackpack(cellIndex);
        }
    }
    // �ƶ����嵽�ڴ���֧������ʱ������
    private void MoveTrapToPocket(int trapIndex)
    {
        // ��ȡԴ�������ݣ������е����壩
        TrapSlotInfo sourceTrap = _trapSlotInfos[trapIndex];
        TrapSlotInfo replacedTrap = null;

        // ���ڴ��Ƿ�����
        if (_tempTrapSlotInfos.Count < MAX_POCKET_TRAP)
        {
            // �ڴ�δ����ֱ�����Դ����
            _tempTrapSlotInfos.Add(sourceTrap);
        }
        else
        {
            // �ڴ����������汻�滻�����壬���滻ΪԴ����
            replacedTrap = _tempTrapSlotInfos[MAX_POCKET_TRAP - 1]; // �������һ������
            _tempTrapSlotInfos[MAX_POCKET_TRAP - 1] = sourceTrap;   // �滻���һ������
        }

        // �ӱ����Ƴ�Դ����
        _trapSlotInfos.RemoveAt(trapIndex);

        // ����б��滻�����壬��ӵ�����ԭλ��
        if (replacedTrap != null)
        {
            _trapSlotInfos.Insert(trapIndex, replacedTrap); // ���뵽ԭ����λ�ã������б��Ȳ��䣩
        }

        // ˢ��UI
        RefreshLists();
    }

// �ƶ����嵽������֧������ʱ������
    private void MoveTrapToBackpack(int trapIndex)
    {
        // ��ȡԴ�������ݣ��ڴ��е����壩
        TrapSlotInfo sourceTrap = _tempTrapSlotInfos[trapIndex];
        TrapSlotInfo replacedTrap = null;

        // ��鱳�������Ƿ�����
        if (_trapSlotInfos.Count < MAX_BACKPACK_TRAP)
        {
            // ����δ����ֱ�����Դ����
            _trapSlotInfos.Add(sourceTrap);
        }
        else
        {
            // �������������汻�滻�����壬���滻ΪԴ����
            replacedTrap = _trapSlotInfos[MAX_BACKPACK_TRAP - 1]; // �������һ������
            _trapSlotInfos[MAX_BACKPACK_TRAP - 1] = sourceTrap;   // �滻���һ������
        }

        // �ӿڴ��Ƴ�Դ����
        _tempTrapSlotInfos.RemoveAt(trapIndex);

        // ����б��滻�����壬��ӵ��ڴ�ԭλ��
        if (replacedTrap != null)
        {
            _tempTrapSlotInfos.Insert(trapIndex, replacedTrap); // ���뵽ԭ����λ�ã������б��Ȳ��䣩
        }

        // ˢ��UI
        RefreshLists();
    }

    // ˢ�������б�
    private void RefreshLists()
    {
        // ˢ�±����б�����+���������仯��
        scrollView.ShowList(_materialSlotInfos.Count + _trapSlotInfos.Count);
        // ˢ�¿ڴ��б�
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
        /*// ��ȡ�������Ļ�ϵ�λ��
        Vector2 mousePosition = Input.mousePosition;
        // �������Ļ����ת��Ϊ Canvas �ľֲ�����
        RectTransform canvasRectTransform = UIController.Instance.uiCanvas.transform as RectTransform;
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePosition, null, out localMousePosition);
        // Ӧ��ƫ������������һ�㣩
        Vector2 offset = new Vector2(10, -10); // ƫ���������Ը�����Ҫ����
        localMousePosition += offset;

        // ������ϸ��������λ��
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