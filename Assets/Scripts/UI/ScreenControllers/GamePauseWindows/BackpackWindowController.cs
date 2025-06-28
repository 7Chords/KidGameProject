using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI;
using UnityEngine;

public class BackpackWindowController : WindowController
{
    private UICircularScrollView scrollView;
    private List<MaterialSlotInfo> _materialSlotInfos;
    private List<TrapSlotInfo> _trapSlotInfos;
    protected override void Awake()
    {
        base.Awake();
        
    }

    protected override void OnPropertiesSet() {
        scrollView = gameObject.GetComponentInChildren<UICircularScrollView>();
        _materialSlotInfos = PlayerBag.Instance.GetMaterialSlots();
        _trapSlotInfos = PlayerBag.Instance.GetTrapSlots();
        scrollView.Init(_materialSlotInfos.Count + _trapSlotInfos.Count,OnCellUpdate,null);
        
    }

    private void OnCellUpdate(GameObject cell,int index)
    {
        CellUI cellUI = cell.GetComponent<CellUI>();
        if (index - 1 < _materialSlotInfos.Count)
        {
            cellUI.SetUIWithMaterial(_materialSlotInfos[index-1]);
        }else if (index - 1 - _materialSlotInfos.Count < _trapSlotInfos.Count)
        {
            cellUI.SetUIWithTrap(_trapSlotInfos[index - 1 - _materialSlotInfos.Count]);
        }
    }
}
