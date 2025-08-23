using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace KidGame.UI
{
    /// <summary>
    /// PlayerWindow属性类
    /// </summary>
    [Serializable]
    public class PopItemContainerProp : PanelProperties
    {
        public List<ISlotInfo> items;//库存背包
        public int row;
        public int column;
        public string title = String.Empty;
        public List<MaterialItem> originItems;
       
        public int maxCount => row * column;

        public PopItemContainerProp()
        {
            items = new List<ISlotInfo>();
            row = 2;
            column = 2;
        }
        
        public PopItemContainerProp(List<ISlotInfo> items, int row, int column,List<MaterialItem> originItems) {
            this.items = items;
            this.row = row;
            this.column = column;
            this.originItems = originItems;
        }

        public PopItemContainerProp(List<ISlotInfo> items, int row, int column, List<MaterialItem> originItems,string title):this(items, row, column,originItems)
        {
            this.title = title;
        }
    }
    [Serializable]
    public class PopItemContainerPanelController : PanelController<PopItemContainerProp>
    {
        private UICircularScrollView scrollView;
        private RectTransform rectTransform;
        private RectTransform scrollViewRectTransform;
        private RectTransform cellRect;
        private RectTransform itemContainerRect;
        private RectTransform viewPortRect;
        private float spacing = 10f;
        private TextMeshProUGUI lab_title;
        private Button btn_getAll;
        public float xspacing = 800f;
        public float yspacing = 800f;
        
        

        protected override void AddListeners()
        {
            
        }

        protected override void RemoveListeners()
        {
            
        }
        
        
        
        protected override void OnPropertiesSet()
        {
            PlayerController.Instance.InputSettings.OnInteractionPressWithoutTime += HidePopPanel;
            base.OnPropertiesSet();
            //GameManager.Instance.GamePause();
            rectTransform = GetComponent<RectTransform>();
            scrollView = transform.Find("ItemContainer/ScrollView").GetComponent<UICircularScrollView>();
            scrollViewRectTransform = scrollView.GetComponent<RectTransform>();
            cellRect = scrollView.m_CellGameObject.GetComponent<RectTransform>();
            lab_title = transform.Find("ItemContainer/lab_title").GetComponent<TextMeshProUGUI>();
            itemContainerRect = transform.Find("ItemContainer").GetComponent<RectTransform>();
            viewPortRect = scrollView.transform.Find("Viewport").GetComponent<RectTransform>();
            // 初始化标题
            InitTitle();
            
            // 设置滚动视图大小
            SetScrollViewSize(Properties);
            
            // 初始化滚动视图
            scrollView.Init(Properties.items.Count, OnContainerCellUpdate, OnContainerCellClick, null);
            
            
        }

        public void HidePopPanel()
        {
            UIController.Instance.HidePanel(ScreenIds.PopItemContainerPanel);
            Signals.Get<CloseBackpackWindowSignal>().Dispatch();
        }
        
        protected override void WhileHiding()
        {
            
            PlayerController.Instance.InputSettings.OnInteractionPressWithoutTime -= HidePopPanel;
            //GameManager.Instance.GameResume();
            
        }
        
        /// <summary>
        /// 初始化标题显示
        /// </summary>
        private void InitTitle()
        {
            if (!string.IsNullOrEmpty(Properties.title))
            {
                lab_title.text = Properties.title;
                lab_title.gameObject.SetActive(true);
            }
            else
            {
                lab_title.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 设置滚动视图大小
        /// </summary>
        public void SetScrollViewSize(PopItemContainerProp properties)
        {
            if (cellRect == null) return;
            
            // 计算滚动视图内容大小（考虑格子大小和间距）
            float cellWidth = cellRect.rect.width;
            float cellHeight = cellRect.rect.height;
            float contentWidth = properties.column * cellWidth + (properties.column) * spacing + 50f;
            float contentHeight = properties.row * cellHeight + (properties.row) * spacing;
            
            // 设置滚动视图大小
            
            scrollViewRectTransform.sizeDelta = new Vector2(contentWidth, contentHeight);
            viewPortRect.sizeDelta = new Vector2(contentWidth, contentHeight);
            itemContainerRect.sizeDelta = new Vector2(contentWidth+xspacing, contentHeight+yspacing);
            rectTransform.sizeDelta = new Vector2(contentWidth+xspacing + 300f, contentHeight+yspacing + 180f);
        }
        
        /// <summary>
        /// 更新容器单元格显示
        /// </summary>
        public void OnContainerCellUpdate(GameObject cell, int index)
        {
            if (index < 0 || index >= Properties.items.Count) return;
            
            CellUI cellUI = cell.GetComponent<CellUI>();
            ISlotInfo slotInfo = Properties.items[index-1];
            
            // 根据物品类型设置单元格显示
            cellUI.SetUIWithGenericSlot(slotInfo);
            
        }

        /// <summary>
        /// 容器单元格点击事件（与背包交换物品）
        /// </summary>
        public void OnContainerCellClick(GameObject cell, int index)
        {
            if (index-1 < 0 || index-1 >= Properties.items.Count) return;
            // 原本的道具栏与背包互换代码移动到了playerBag.cs
            PlayerBag.Instance.MoveContainerItemToBackBag(index - 1,Properties);
            RefreshLists();
            UIHelper.Instance.HideItemDetail();
        }
        private void RefreshLists()
        {
            scrollView.ShowList(Properties.items.Count);
            // 通知背包UI刷新
            Signals.Get<RefreshBackpackSignal>().Dispatch();
        }
        
        /// <summary>
        /// 查找背包中可交换的物品索引
        /// </summary>
        private int FindBackpackSwapIndex(ISlotInfo containerItem)
        {
            // 1. 优先查找相同类型的物品（可堆叠）
            for (int i = 0; i < PlayerBag.Instance.BackBag.Count; i++)
            {
                ISlotInfo backpackItem = PlayerBag.Instance.BackBag[i];
                if (backpackItem.ItemData == containerItem.ItemData)
                {
                    return i;
                }
            }
            
            // 2. 查找空槽位
            for (int i = 0; i < PlayerBag.Instance.BackBag.Count; i++)
            {
                if (PlayerBag.Instance.BackBag[i].ItemData == null)
                {
                    return i;
                }
            }
            
            return -1; // 背包已满，无法交换
        }
        /// <summary>
        /// 一键拿取所有物品
        /// </summary>
        public void UI_OnGetAllClick()
        {
            // 将容器中所有物品移动到背包
            foreach (var item in Properties.items)
            {
                if (item.ItemData != null)
                {
                    if (PlayerBag.Instance.AddItemToCombineBag(item.ItemData.Id, item.ItemData.UseItemType,
                            item.Amount))
                    {
                        UIHelper.Instance.ShowOneTip(new TipInfo("背包已满", PlayerController.Instance.transform.position));
                        break;
                    }
                }
            }
            
            // 清空容器
            Properties.items.Clear();
            Properties.originItems.Clear();
            RefreshContainer();
            
            // 通知背包UI刷新
            Signals.Get<RefreshBackpackSignal>().Dispatch();
            
        }
        
        /// <summary>
        /// 刷新容器列表
        /// </summary>
        private void RefreshContainer()
        {
            scrollView.ShowList(Properties.items.Count);
        }

        

        
        
    }
    
}