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

    public class PopItemPanelCloseSignal : ASignal
    {
        
    }
    
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


        // 添加状态变量
        private bool _isPanelActive = false;
        protected override void AddListeners()
        {
            //base.AddListeners();
            
        }

        protected override void RemoveListeners()
        {
            //PlayerController.Instance.InputSettings.OnInteractionPressWithoutTime -= HidePopPanel;
            
        }
        
        
        
        protected override void OnPropertiesSet()
        {
            
            base.OnPropertiesSet();
            
            _isPanelActive = true; // 标记面板为激活状态
            //todo
            //一键拾取失效
            MsgCenter.RegisterMsgAct(MsgConst.ON_UI_INTERACTION_PRESS, UI_OnGetAllClick);
            Signals.Get<PopItemPanelCloseSignal>().AddListener(HidePopPanel);
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
            if (!_isPanelActive) return; // 状态校验
            UIController.Instance.HidePanel(ScreenIds.PopItemContainerPanel);
            PlayerBag.Instance.TargetList = null;
            //Signals.Get<CloseBackpackWindowSignal>().Dispatch();
        }
        
        protected override void WhileHiding()
        {
            _isPanelActive = false;
            MsgCenter.UnregisterMsgAct(MsgConst.ON_UI_INTERACTION_PRESS, UI_OnGetAllClick);
            Signals.Get<PopItemPanelCloseSignal>().RemoveListener(HidePopPanel);
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
            if(Properties.items.Count == 0) return;
            bool isGetAll = true;
            List<int> deleteIndices = new List<int>();
            for (int i = 0; i < Properties.items.Count; i++)
            {
                ISlotInfo slotInfo = Properties.items[i];
                if (slotInfo.ItemData != null)
                {
                    if (!PlayerBag.Instance.AddItemToCombineBag(slotInfo.ItemData.Id, slotInfo.ItemData.UseItemType,
                            slotInfo.Amount))
                    {
                        UIHelper.Instance.ShowOneTip(new TipInfo("背包已满", PlayerController.Instance.transform.position));
                        isGetAll = false;
                        break;
                    }
                    else
                    {
                        deleteIndices.Add(i);
                    }
                }
            }

            // 第二遍：按倒序删除已拿取的物品，避免索引错误
            if (deleteIndices.Count > 0)
            {
                // 按索引倒序排序，确保删除时不影响后续元素索引
                deleteIndices.Sort((a, b) => b.CompareTo(a));
        
                foreach (int index in deleteIndices)
                {
                    if (index >= 0 && index < Properties.items.Count)
                    {
                        Properties.items.RemoveAt(index);
                    }
            
                    if (index >= 0 && index < Properties.originItems.Count)
                    {
                        Properties.originItems.RemoveAt(index);
                    }
                }
            }
            // 将容器中所有物品移动到背包
            /*foreach (var item in Properties.items)
            {
                if (item.ItemData != null)
                {
                    if (!PlayerBag.Instance.AddItemToCombineBag(item.ItemData.Id, item.ItemData.UseItemType,
                            item.Amount))
                    {
                        UIHelper.Instance.ShowOneTip(new TipInfo("背包已满", PlayerController.Instance.transform.position));
                        isGetAll = false;
                        break;
                    }
                }
            }

            if (isGetAll)
            {
                // 清空容器
                Properties.items.Clear();
                Properties.originItems.Clear();
                UIController.Instance.HidePanel(ScreenIds.PopItemContainerPanel);
            }*/
            
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