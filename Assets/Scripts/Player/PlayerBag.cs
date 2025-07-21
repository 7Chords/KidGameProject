using System;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.Interface;
using KidGame.UI;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// 背包物品基类接口，用于统一不同类型的物品数据
    /// </summary>
    public interface BagItemInfoBase
    {
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

        public List<ISlotInfo> _tempBag = new();                  // 道具栏，最多4个
        public List<TrapSlotInfo> _trapBag = new();               // 陷阱背包
        public List<MaterialSlotInfo> _materialBag = new();       // 材料背包
        public List<WeaponSlotInfo> _weaponBag = new();           // 武器背包

        #endregion

        #region 当前选中道具索引逻辑

        public event Action<TrapData> SelectTrapAction;

        private int _selectedTrapIndex = 0;

        public int SelectedIndex
        {
            get => _selectedTrapIndex;
            set
            {
                int newIndex = _tempBag.Count > 0 ? Mathf.Clamp(value, 0, _tempBag.Count - 1) : 0;
                if (_selectedTrapIndex == newIndex) return;

                _selectedTrapIndex = newIndex;

                var selectedSlot = _tempBag[_selectedTrapIndex];
                string itemName = selectedSlot.ItemData switch
                {
                    TrapData trap => trap.trapName,
                    WeaponData weapon => weapon.name,
                    FoodData food => food.foodName,
                    MaterialData mat => mat.materialName,
                    _ => "未知物品"
                };

                UIHelper.Instance.ShowOneTip(new TipInfo($"已选择: {itemName}", PlayerController.Instance.gameObject));
                OnTrapBagUpdated?.Invoke();
            }
        }

        public ISlotInfo GetSelectedTempItem()
        {
            return _tempBag.Count > 0 ? _tempBag[_selectedTrapIndex] : null;
        }

        #endregion

        #region 事件定义

        public event Action OnTrapBagUpdated;

        #endregion

        #region 注册与反注册

        public void Init()
        {
            PlayerUtil.Instance.RegPlayerPickItem(PlayerGetItem);
        }

        public void Discard()
        {
            PlayerUtil.Instance.UnregPlayerPickItem(PlayerGetItem);
        }

        #endregion

        #region 公共方法接口

        public List<TrapSlotInfo> GetTrapSlots() => _trapBag;
        public List<MaterialSlotInfo> GetMaterialSlots() => _materialBag;
        public List<ISlotInfo> GetTempSlots() => _tempBag;

        public void TrapBagUpdated() => OnTrapBagUpdated?.Invoke();

        /// <summary>
        /// 加载存档中的背包数据
        /// </summary>
        public void LoadBagData(List<TrapSlotInfo> trapSlots, List<MaterialSlotInfo> materialSlots)
        {
            // _trapBag = trapSlots ?? new List<TrapSlotInfo>();
            // _materialBag = materialSlots ?? new List<MaterialSlotInfo>();
            OnTrapBagUpdated?.Invoke();
        }

        /// <summary>
        /// 尝试从背包中删除指定的陷阱和材料
        /// </summary>
        public bool TryDelItemInBag(List<TrapSlotInfo> requireTrapSlots = null, List<MaterialSlotInfo> requireMaterialSlots = null)
        {
            if (requireTrapSlots == null && requireMaterialSlots == null) return true;

            if (requireTrapSlots != null)
            {
                foreach (var trapSlot in requireTrapSlots)
                {
                    var existingSlot = _trapBag.Find(x => x.trapData.id == trapSlot.trapData.id);
                    if (existingSlot != null && existingSlot.amount >= trapSlot.amount)
                    {
                        existingSlot.amount -= trapSlot.amount;
                    }
                    else return false;
                }
            }

            if (requireMaterialSlots != null)
            {
                foreach (var materialSlot in requireMaterialSlots)
                {
                    var existingSlot = _materialBag.Find(x => x.materialData.id == materialSlot.materialData.id);
                    if (existingSlot != null && existingSlot.amount >= materialSlot.amount)
                    {
                        existingSlot.amount -= materialSlot.amount;
                    }
                    else return false;
                }
            }

            OnTrapBagUpdated?.Invoke();
            return true;
        }

        #endregion

        #region 拾取与使用物品逻辑

        private void PlayerGetItem(IPickable iPickable)
        {
            if (iPickable == null) return;

            switch (iPickable)
            {
                case TrapBase trapBase:
                    AddToBag(new TrapSlotInfo(trapBase.TrapData, 1));
                    break;
                case MaterialBase materialBase:
                    AddToBag(new MaterialSlotInfo(materialBase.materialData, 1));
                    break;
                case WeaponBase weaponBase:
                    AddToBag(new WeaponSlotInfo(weaponBase.weaponData, 1));
                    break;
            }
        }

        public void AddToBag(ISlotInfo newItem)
        {
            var existing = _tempBag.Find(x => x.ItemData == newItem.ItemData);
            if (existing != null)
            {
                existing.Amount += newItem.Amount;
            }
            else if (_tempBag.Count < 4)
            {
                _tempBag.Add(newItem);
            }
            else
            {
                // 道具栏已满，转移至背包
                switch (newItem)
                {
                    case TrapSlotInfo trapSlot:
                        AddToTrapBag(trapSlot);
                        break;
                    case MaterialSlotInfo materialSlot:
                        AddToMaterialBag(materialSlot);
                        break;
                    case WeaponSlotInfo weaponSlot:
                        AddToWeaponBag(weaponSlot);
                        break;
                    default:
                        UIHelper.Instance.ShowOneTip(new TipInfo("无法识别的物品类型", PlayerController.Instance.gameObject));
                        return;
                }

                UIHelper.Instance.ShowOneTip(new TipInfo("道具栏已满，物品已放入背包", PlayerController.Instance.gameObject));
            }

            OnTrapBagUpdated?.Invoke();
        }
        
        // 具体的添加方法，但是真的要这么麻烦吗
        private void AddToTrapBag(TrapSlotInfo newSlot)
        {
            var existing = _trapBag.Find(x => x.trapData.id == newSlot.trapData.id);
            if (existing != null)
            {
                existing.amount += newSlot.amount;
            }
            else
            {
                _trapBag.Add(newSlot);
            }
        }

        private void AddToMaterialBag(MaterialSlotInfo newSlot)
        {
            var existing = _materialBag.Find(x => x.materialData.id == newSlot.materialData.id);
            if (existing != null)
            {
                existing.amount += newSlot.amount;
            }
            else
            {
                _materialBag.Add(newSlot);
            }
        }

        private void AddToWeaponBag(WeaponSlotInfo newSlot)
        {
            var existing = _weaponBag.Find(x => x.weaponData.id == newSlot.weaponData.id);
            if (existing != null)
            {
                existing.amount += newSlot.amount;
            }
            else
            {
                _weaponBag.Add(newSlot);
            }
        }

        /// <summary>
        /// 使用陷阱
        /// </summary>
        public bool UseTrap(ISlotInfo slot, Vector3 position, Quaternion rotation)
        {
            if (slot is TrapSlotInfo trapSlot)
            {
                if (!PlayerController.Instance.GetCanPlaceTrap())
                {
                    UIHelper.Instance.ShowOneTip(new TipInfo("这里无法放置陷阱", PlayerController.Instance.gameObject));
                    return false;
                }

                GameObject newTrap = TrapFactory.CreateEntity(trapSlot.trapData, position);
                if (newTrap != null)
                {
                    newTrap.transform.rotation = rotation;
                    DecreaseSlot(slot);
                    return true;
                }
            }
            return false;
        }

        public bool UseWeapon(ISlotInfo slot, PlayerController player, Vector3 position) => false;
        public bool UseFood(ISlotInfo slot, PlayerController player) => false;
        public bool UseMaterial(ISlotInfo slot, PlayerController player) => false;

        private void DecreaseSlot(ISlotInfo slot)
        {
            slot.Amount--;
            if (slot.Amount <= 0)
            {
                _tempBag.Remove(slot);
                _selectedTrapIndex = Mathf.Clamp(_selectedTrapIndex, 0, _tempBag.Count - 1);
            }

            OnTrapBagUpdated?.Invoke();
        }

        #endregion

        #region 道具栏与背包互换位置

        public void MoveTrapToPocket(int trapIndex)
        {
            if (trapIndex < 0 || trapIndex >= _trapBag.Count || _tempBag.Count >= 4)
                return;

            var item = _trapBag[trapIndex];
            _trapBag.RemoveAt(trapIndex);
            _tempBag.Add(item);

            OnTrapBagUpdated?.Invoke();
        }

        public void MoveTrapToBackpack(int pocketIndex)
        {
            if (pocketIndex < 0 || pocketIndex >= _tempBag.Count || _trapBag.Count >= 10)
                return;
            
            var item = _tempBag[pocketIndex];
            _tempBag.RemoveAt(pocketIndex);
            // todo.否则需判断类型
            _trapBag.Add((TrapSlotInfo)item);

            OnTrapBagUpdated?.Invoke();
        }
        
        #endregion
    }
}
