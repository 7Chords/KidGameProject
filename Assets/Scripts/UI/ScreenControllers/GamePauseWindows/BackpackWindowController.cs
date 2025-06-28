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
    private List<MaterialSlotInfo> _materialSlotInfos;
    private List<TrapSlotInfo> _trapSlotInfos;
    private GameObject detailPanel;
    private TextMeshProUGUI detailText;
    
    protected override void Awake()
    {
        base.Awake();
        detailPanel = transform.Find("BackPag/ScrollView/DetailPanel").gameObject;
        detailText = transform.Find("BackPag/ScrollView/DetailPanel/DetailText").GetComponent<TextMeshProUGUI>();
        detailPanel.SetActive(false);
        Signals.Get<ShowItemDetailSignal>().AddListener(OnShowItemDetail);
        Signals.Get<HideItemDetailSignal>().AddListener(OnHideItemDetail);
    }

    protected override void OnPropertiesSet() {
        scrollView = gameObject.GetComponentInChildren<UICircularScrollView>();
        _materialSlotInfos = PlayerBag.Instance.GetMaterialSlots();
        _trapSlotInfos = PlayerBag.Instance.GetTrapSlots();
        scrollView.Init(_materialSlotInfos.Count + _trapSlotInfos.Count,OnCellUpdate,null);
        
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Signals.Get<ShowItemDetailSignal>().RemoveListener(OnShowItemDetail);
        Signals.Get<HideItemDetailSignal>().RemoveListener(OnHideItemDetail);
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
        
        detailPanel.transform.position = cellUI.transform.position + new Vector3(1.5f, -1.5f, 0f);
        detailText.text = cellUI.detailText;
        detailPanel.SetActive(true);
    }

    private void OnHideItemDetail()
    {
        detailPanel.SetActive(false);
    }
}
