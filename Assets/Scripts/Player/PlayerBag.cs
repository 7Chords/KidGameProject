using KidGame.UI;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{

    /// <summary>
    /// 背包物品基类接口，用于统一不同类型的物品数据
    /// </summary>
    public interface BagItemInfoBase
    {
        string Id { get; }
        UseItemType UseItemType { get; }
    }

    /// <summary>
    /// 背包槽位信息接口
    /// </summary>
    public interface ISlotInfo
    {
        BagItemInfoBase ItemData { get; }
        int Amount { get; set; }
    }

    #region 各类物品槽位信息

    [Serializable]
    public class TrapSlotInfo : ISlotInfo
    {
        public TrapData trapData;
        public int amount;

        public TrapSlotInfo(TrapData trapData, int amount)
        {
            this.trapData = trapData;
            this.amount = amount;
        }

        public BagItemInfoBase ItemData => trapData;
        public int Amount { get => amount; set => amount = value; }
    }

    [Serializable]
    public class WeaponSlotInfo : ISlotInfo
    {
        public WeaponData weaponData;
        public int amount;

        public WeaponSlotInfo(WeaponData weaponData, int amount)
        {
            this.weaponData = weaponData;
            this.amount = amount;
        }

        public BagItemInfoBase ItemData => weaponData;
        public int Amount { get => amount; set => amount = value; }
    }

    [Serializable]
    public class MaterialSlotInfo : ISlotInfo
    {
        public MaterialData materialData;
        public int amount;

        public MaterialSlotInfo(MaterialData materialData, int amount)
        {
            this.materialData = materialData;
            this.amount = amount;
        }

        public BagItemInfoBase ItemData => materialData;
        public int Amount { get => amount; set => amount = value; }
    }

    #endregion

    /// <summary>
    /// 玩家背包核心类（单例模式）
    /// </summary>
    public class PlayerBag : Singleton<PlayerBag>
    {
        #region 背包结构
        //TIPS:如果在方法名中看到combineBag的名称 就是上面两个列表都会去查找
        //比如要删除某个id的道具就是要两个都去找 加入先加道具栏再加背包
        public List<ISlotInfo> QuickAccessBag;//道具栏，最多4个
        public List<ISlotInfo> BackBag;//库存背包

        //当同时开启背包和其他容器时，点击物品会出现小菜单栏，可选择到底移动到哪个容器，
        //这个引用就是用来存储其他容器的那个list的引用
        private List<ISlotInfo> targetList = null;
        
        public List<ISlotInfo> TargetList{ get => targetList; set => targetList = value; } 
        

        #endregion
        
        #region 当前选中道具索引逻辑


        private int _selectedIndex = 0;

        //SelectedIndex永远只会被鼠标滚轮进行设置 在GamePlayPanelController中进行设置
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {

                int newIndex = Mathf.Clamp(value, 0, GlobalValue.QUICK_ACCESS_BAG_CAPACITY);

                _selectedIndex = newIndex;

                if(_selectedIndex < QuickAccessBag.Count)
                {
                    var selectedSlot = QuickAccessBag[_selectedIndex];
                    if (selectedSlot != null) MsgCenter.SendMsg(MsgConst.ON_SELECT_ITEM, selectedSlot);

                    string itemName = selectedSlot.ItemData switch
                    {
                        TrapData trap => trap.trapName,
                        WeaponData weapon => weapon.name,
                        FoodData food => food.foodName,
                        MaterialData mat => mat.materialName,
                        _ => "未知物品"
                    };

                    UIHelper.Instance.ShowOneTip(new TipInfo($"已选择: {itemName}", PlayerController.Instance.transform.position));
                }
                else
                {
                    MsgCenter.SendMsg(MsgConst.ON_SELECT_ITEM, null);
                }
                MsgCenter.SendMsgAct(MsgConst.ON_QUICK_BAG_UPDATE);
            }
        }

        public ISlotInfo GetSelectedQuickAccessItem()
        {
            return _selectedIndex < QuickAccessBag.Count ? QuickAccessBag[_selectedIndex] : null;
        }

        #endregion


        #region 注册与反注册

        public void Init()
        {
            MsgCenter.RegisterMsg(MsgConst.ON_PICK_ITEM, PlayerGetOneItem);
            QuickAccessBag = new List<ISlotInfo>(GlobalValue.QUICK_ACCESS_BAG_CAPACITY);
            BackBag = new List<ISlotInfo>();
        }

        public void Discard()
        {
            MsgCenter.UnregisterMsg(MsgConst.ON_PICK_ITEM, PlayerGetOneItem);
        }

        #endregion

        #region 公共方法接口
        public List<ISlotInfo> GetQuickAccessBag() => QuickAccessBag;
        
        

        /// <summary>
        /// 加载存档中的背包数据 todo:guihuala
        /// </summary>
        public void LoadBagData(List<TrapSlotInfo> trapSlots, List<MaterialSlotInfo> materialSlots)
        {
            // _trapBag = trapSlots ?? new List<TrapSlotInfo>();
            // _materialBag = materialSlots ?? new List<MaterialSlotInfo>();
            MsgCenter.SendMsgAct(MsgConst.ON_QUICK_BAG_UPDATE);
        }




        /// <summary>
        /// 尝试从背包(包括库存和道具栏)中删除指定个数的道具
        /// </summary>
        /// <param name="itemId">道具id</param>
        /// <param name="delAmount">要删除的数量</param>
        /// <returns>是否移除成功</returns>
        public bool DeleteItemInCombineBag(string itemId, int delAmount)
        {
            if (string.IsNullOrEmpty(itemId) || delAmount <= 0) return false;
            bool slotInBackBag = true;
            ISlotInfo slotInfo = BackBag.Find(X => X.ItemData.Id == itemId);
            if (slotInfo == null)
            {
                slotInBackBag = false;
                slotInfo = QuickAccessBag.Find(x => x.ItemData.Id == itemId);
            }
            if (slotInfo == null) return false;
            slotInfo.Amount = Mathf.Max(0, slotInfo.Amount - delAmount);
            if(slotInfo.Amount == 0)
            {
                if (slotInBackBag) BackBag.Remove(slotInfo);
                else 
                {
                    QuickAccessBag.Remove(slotInfo);
                    if(SelectedIndex >= QuickAccessBag.Count)
                    {
                        MsgCenter.SendMsg(MsgConst.ON_SELECT_ITEM, null);
                    }
                }
            }
            MsgCenter.SendMsgAct(MsgConst.ON_QUICK_BAG_UPDATE);
            return true;
        }

        /// <summary>
        /// 检查背包是否有一定数量的某个物品
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="delAmount"></param>
        /// <returns></returns>
        public bool CheckItemEnoughInCombineBag(string itemId, int delAmount)
        {
            if (string.IsNullOrEmpty(itemId) || delAmount <= 0) return false;
            ISlotInfo slotInfo = BackBag.Find(X => X.ItemData.Id == itemId);
            if (slotInfo == null)
            {
                slotInfo = QuickAccessBag.Find(X => X.ItemData.Id == itemId);
            }
            if (slotInfo == null) return false;
            if (slotInfo.Amount < delAmount) return false;
            return true;
        }

        /// <summary>
        /// 添加物品到背包（先加到道具栏 满了才到背包）
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="addAmount"></param>
        public bool AddItemToCombineBag(string itemId, UseItemType itemType, int addAmount)
        {
            var existing = QuickAccessBag.Find(x => x.ItemData.Id == itemId);
            if(existing == null)
            {
                existing = BackBag.Find(x => x.ItemData.Id == itemId);
            }
            if (existing != null)
            {
                existing.Amount += addAmount;
            }
            else if (QuickAccessBag.Count < GlobalValue.QUICK_ACCESS_BAG_CAPACITY)
            {
                switch (itemType)
                {
                    case UseItemType.trap:
                        QuickAccessBag.Add(new TrapSlotInfo(SoLoader.Instance.GetTrapDataById(itemId), addAmount));
                        break;
                    case UseItemType.Material:
                        QuickAccessBag.Add(new MaterialSlotInfo(SoLoader.Instance.GetMaterialDataDataById(itemId), addAmount));
                        break;
                    case UseItemType.weapon:
                        QuickAccessBag.Add(new WeaponSlotInfo(SoLoader.Instance.GetWeaponDataById(itemId), addAmount));
                        break;
                    //todo:food
                    default:
                        break;
                }
                //当前选中的物品栏从无到有 马上刷新
                if(QuickAccessBag.Count == SelectedIndex + 1)
                {
                    MsgCenter.SendMsg(MsgConst.ON_SELECT_ITEM, QuickAccessBag[SelectedIndex]);
                }
            }
            else if(BackBag.Count<GlobalValue.BACKPACK_CAPACITY)
            {
                // 道具栏已满，转移至背包
                switch (itemType)
                {
                    case UseItemType.trap:
                        BackBag.Add(new TrapSlotInfo(SoLoader.Instance.GetTrapDataById(itemId), 1));
                        break;
                    case UseItemType.Material:
                        BackBag.Add(new MaterialSlotInfo(SoLoader.Instance.GetMaterialDataDataById(itemId), 1));
                        break;
                    case UseItemType.weapon:
                        BackBag.Add(new WeaponSlotInfo(SoLoader.Instance.GetWeaponDataById(itemId), 1));
                        break;
                    //todo:food
                    default:
                        break;
                }
            }
            else
            {
                return false;
            }

            MsgCenter.SendMsgAct(MsgConst.ON_QUICK_BAG_UPDATE);
            return true;
        }

        #endregion

        #region 拾取与使用物品逻辑

        private void PlayerGetOneItem(object[] objs)
        {
            if (objs == null || objs.Length == 0) return;

            string itemId = objs[0] as string;
            UseItemType itemType = (UseItemType)objs[1];

            if (string.IsNullOrEmpty(itemId)) return;

            AddItemToCombineBag(itemId, itemType, 1);
        }

        /// <summary>
        /// 使用陷阱
        /// </summary>
        //public bool UseTrap(ISlotInfo slot, Vector3 position, Quaternion rotation)
        //{
        //    if (!PlayerController.Instance.GetCanPlaceTrap())
        //    {
        //        UIHelper.Instance.ShowOneTip(new TipInfo("这里无法放置陷阱", PlayerController.Instance.gameObject));
        //        return false;
        //    }
        //    return true;
        //}

        public bool UseWeaponShortClick(ISlotInfo slot, Vector3 position, Quaternion rotation)
        {
            // 投掷物使用的时候需要用特殊的逻辑 其他物品
            if (slot is WeaponSlotInfo)
            {
                GameObject curWeapon = PlayerController.Instance.GetCurWeapon();
                WeaponBase curWeaponScript = curWeapon.GetComponent<WeaponBase>();
                if (curWeaponScript.weaponData.weaponType == 1 && curWeaponScript.weaponData.useType == 0)
                {
                    // 如果是远程的消耗性物品:
                    if (curWeapon != null)
                    {
                    // 不在手上 区别于选择物品的时候 把这个生成的武器直接开始主要逻辑
                        curWeapon.transform.SetParent(null);
                        curWeapon.transform.rotation = rotation;
                        curWeaponScript.SetIsNotUse(false);
                        DeleteItemInCombineBag(slot.ItemData.Id, 1);
                        // 如果是投掷类的需要使用后持续存在的 那么去掉这个引用
                        PlayerController.Instance.SetWeaponAndWeaponDataReference2Null();
                        return true;
                    }
                }
            }
            return false;
        }

        // 我发现长按的道具好像不需要在use state做逻辑
        public bool UseWeaponLongPress(ISlotInfo slot)
        {
            if (slot != null && slot is WeaponSlotInfo)
            {

                
                return true;
            }
            return false;
        }

        public bool UseFood(ISlotInfo slot, PlayerController player) => false;
        public bool UseMaterial(ISlotInfo slot, PlayerController player) => false;

        #endregion
       



        #region 道具栏与背包互换位置

        public void MoveItemToQuickAccessBag(int selectIndex)
        {
            if (selectIndex < 0 || selectIndex >= BackBag.Count || QuickAccessBag.Count >= GlobalValue.QUICK_ACCESS_BAG_CAPACITY)
                return;

            var item = BackBag[selectIndex];
            BackBag.RemoveAt(selectIndex);
            QuickAccessBag.Add(item);
            MsgCenter.SendMsgAct(MsgConst.ON_QUICK_BAG_UPDATE);

        }

        public void MoveItemToBackBag(int selectIndex)
        {
            if (selectIndex < 0 || selectIndex >= QuickAccessBag.Count || BackBag.Count >= GlobalValue.BACKPACK_CAPACITY)
                return;
            
            var item = QuickAccessBag[selectIndex];
            QuickAccessBag.RemoveAt(selectIndex);
            BackBag.Add(item);
            MsgCenter.SendMsgAct(MsgConst.ON_QUICK_BAG_UPDATE);

        }

        #endregion

        #region 道具栏与家具容器互换位置

        public void MoveItemToItemContainer(int selectIndex,PopItemContainerProp prop)
        {
            if (selectIndex < 0 || selectIndex >= BackBag.Count || prop.items.Count >= prop.maxCount)
                return;
            
            var item = BackBag[selectIndex];
            //if(!(item.ItemData is MaterialData))return;
            BackBag.RemoveAt(selectIndex);
            prop.originItems.Add(new MaterialItem(item.ItemData as MaterialData,item.Amount));
            targetList.Add(item);
            //OnQuickAccessBagUpdated?.Invoke();
        }

        public void MoveContainerItemToBackBag(int selectIndex,PopItemContainerProp prop)
        {
            if (selectIndex < 0 || selectIndex >= prop.maxCount || BackBag.Count >= GlobalValue.BACKPACK_CAPACITY)
                return;
            
            var item = prop.items[selectIndex];
            //if(!(item.ItemData is MaterialData))return;
            AddItemToCombineBag(item.ItemData.Id, item.ItemData.UseItemType, item.Amount);
            //BackBag.Add(item);
            prop.originItems.RemoveAt(selectIndex);
            prop.items.RemoveAt(selectIndex);
            

            //OnQuickAccessBagUpdated?.Invoke();
        }
        
        #endregion
    }
}
