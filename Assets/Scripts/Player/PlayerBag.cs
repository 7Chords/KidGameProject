using System;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.Interface;
using KidGame.UI;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// 背包物品基类
    /// </summary>
    
    public interface BagItemInfoBase
    {
        UseItemType UseItemType { get; }
    }


    #region 单个物品的信息结构
    
    [Serializable]
    public class TrapSlotInfo : BagItemInfoBase
    {
        public TrapData trapData;
        public int amount;

        public TrapSlotInfo(TrapData trapData, int amount)
        {
            this.trapData = trapData;
            this.amount = amount;
        }

        public UseItemType UseItemType => UseItemType.trap;
    }

    [Serializable]
    public class MaterialSlotInfo : BagItemInfoBase
    {
        public MaterialData materialData;
        public int amount;

        public MaterialSlotInfo(MaterialData materialData, int amount)
        {
            this.materialData = materialData;
            this.amount = amount;
        }
        public UseItemType UseItemType => UseItemType.Material;
    }
    [Serializable]
    public class WeaponSlotInfo
    {
        public WeaponData weaponData;
        public int amount;

        public WeaponSlotInfo(WeaponData weaponData, int amount)
        {
            this.weaponData = weaponData;
            this.amount = amount;
        }
        public UseItemType UseItemType => UseItemType.weapon;
    }
    #endregion

    public class PlayerBag : Singleton<PlayerBag>
    {
        #region 物品列表

        // 当前捡到的物品、以及从存档中读取数据、保存数据的逻辑
        // 陷阱道具栏，最大为4个道具，拾取到的道具先放入道具栏，已满才放入背包
        public List<TrapSlotInfo> _tempTrapBag = new List<TrapSlotInfo>();

        // 陷阱背包
        public List<TrapSlotInfo> _trapBag = new List<TrapSlotInfo>();
        // 材料背包
        public List<MaterialSlotInfo> _materialBag = new List<MaterialSlotInfo>();
        // 手持背包
        public List<WeaponSlotInfo>  _weaponBag = new List<WeaponSlotInfo>();

        #endregion

        #region 选中

        public event Action<TrapData> SelectTrapAction;

        private int _selectedTrapIndex = 0; // 当前选中的陷阱索引
        
        public int SelectedTrapIndex
        {
            get => _selectedTrapIndex;
            set
            {
                int newIndex = _tempTrapBag.Count > 0 ? Mathf.Clamp(value, 0, _tempTrapBag.Count - 1) : 0;
        
                if (_selectedTrapIndex == newIndex) return;
        
                _selectedTrapIndex = newIndex;
                if (_tempTrapBag.Count > _selectedTrapIndex)
                {
                    string trapName = _tempTrapBag[_selectedTrapIndex].trapData.trapName;
                    UIHelper.Instance.ShowOneTip(new TipInfo($"已选择: {trapName}", PlayerController.Instance.gameObject));
                }
                OnTrapBagUpdated?.Invoke();
                SelectTrapAction?.Invoke(_tempTrapBag[_selectedTrapIndex].trapData);
            }
        }

        #endregion

        #region 背包事件

        public event Action OnTrapBagUpdated;

        #endregion

        #region 注册事件

        public void Init()
        {
            PlayerUtil.Instance.RegPlayerPickItem(PlayerPickItem);
        }

        public void Discard()
        {
            PlayerUtil.Instance.UnregPlayerPickItem(PlayerPickItem);
        }

        #endregion

        #region 外部调用方法

        public List<TrapSlotInfo> GetTrapSlots()
        {
            return _trapBag;
        }

        public List<MaterialSlotInfo> GetMaterialSlots()
        {
            return _materialBag;
        }

        public void TrapBagUpdated()
        {
            OnTrapBagUpdated?.Invoke();
        }

        // 加载背包数据
        public void LoadBagData(List<TrapSlotInfo> trapSlots, List<MaterialSlotInfo> materialSlots)
        {
            // _trapBag = trapSlots ?? new List<TrapSlotInfo>();
            // _materialBag = materialSlots ?? new List<MaterialSlotInfo>();

            OnTrapBagUpdated?.Invoke();
        }
        
        public bool TryDelItemInBag(List<TrapSlotInfo> requireTrapSlots = null, List<MaterialSlotInfo> requireMaterialSlots = null)
        {
            if(requireTrapSlots == null&&requireMaterialSlots==null) return true;
            if(requireTrapSlots != null)
            {
                foreach (var trapSlot in requireTrapSlots)
                {
                    var existingSlot = _trapBag.Find(x => x.trapData.id == trapSlot.trapData.id);
                    if (existingSlot != null&&existingSlot.amount >= trapSlot.amount)
                    {
                        existingSlot.amount-= trapSlot.amount;
                        OnTrapBagUpdated?.Invoke();
                    }else
                        return false;
                }
            }

            if (requireMaterialSlots != null)
            {
                foreach (var materialSlot in requireMaterialSlots)
                {
                    var existingSlot = _materialBag.Find(x => x.materialData.id == materialSlot.materialData.id);
                    if (existingSlot != null&&existingSlot.amount >= materialSlot.amount)
                    {
                        existingSlot.amount-= materialSlot.amount;
                        OnTrapBagUpdated?.Invoke();
                    }else
                        return false;
                }
            }
            
            return true;
        }
        

        // 获取当前选中的陷阱
        public TrapSlotInfo GetSelectedTrap()
        {
            if (_trapBag.Count == 0) return null;
            return _trapBag[SelectedTrapIndex];
        }

        #endregion

        #region 基本方法

        private void PlayerPickItem(IPickable iPickable)
        {
            if (iPickable == null) return;
        
            if (iPickable is TrapBase)
            {
                TrapData trapData = (iPickable as TrapBase).TrapData;
                if (trapData == null) return;
                
                if (!TryAddToTempTrapBag(trapData))
                {
                    // 道具栏已满，放入背包
                    TrapSlotInfo trapSlotInfo = _trapBag.Find(x => x.trapData.id == trapData.id);
                    if (trapSlotInfo == null)
                    {
                        _trapBag.Add(new TrapSlotInfo(trapData, 1));
                    }
                    else
                    {
                        trapSlotInfo.amount++;
                    }
                }
            
                OnTrapBagUpdated?.Invoke();
            }
            else if (iPickable is MaterialBase)
            {
                MaterialData materialData = (iPickable as MaterialBase).materialData;
                if (materialData == null) return;
                MaterialSlotInfo materialSlotInfo =
                    _materialBag.Find(x => x.materialData.id == materialData.id);
                if (materialSlotInfo == null)
                {
                    _materialBag.Add(new MaterialSlotInfo(materialData, 1));
                }
                else
                {
                    materialSlotInfo.amount++;
                }
            }
        }

        private bool PlaceGroundTrap(TrapSlotInfo trapToPlace, Vector3 position, Quaternion rotation)
        {
            if (!PlayerController.Instance.GetCanPlaceTrap())
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("这里无法放置陷阱", PlayerController.Instance.gameObject));
                return false;
            }
            GameObject newTrap = TrapFactory.CreateEntity(trapToPlace.trapData, position);
            if (newTrap != null)
            {
                newTrap.transform.rotation = rotation;
                return true;
            }

            return false;
        }

        //使用的道具为手持道具：
        private bool UseThrowHandWeapon(PlayerController player, TrapSlotInfo trapToPlace, Vector3 position, Vector3 mousePosition)
        {
            GameObject newTrap = TrapFactory.CreateEntity(trapToPlace.trapData, position);
            if (newTrap != null)
            {
                return true;
            }

            return false;
        }


        #endregion
        
        #region 道具栏方法
        
        public bool TryAddToTempTrapBag(TrapData trapData)
        {
            var existingSlot = _tempTrapBag.Find(x => x.trapData.id == trapData.id);

            if (existingSlot != null)
            {
                existingSlot.amount++;
                OnTrapBagUpdated?.Invoke();
                return true;
            }

            // 如果没有相同陷阱且还有空位
            if (_tempTrapBag.Count < 4)
            {
                _tempTrapBag.Add(new TrapSlotInfo(trapData, 1));
                OnTrapBagUpdated?.Invoke();
                return true;
            }

            return false;
        }
        
        
        public bool TryUseTrapFromTempBag(int index, PlayerController player, Vector3 position, Quaternion rotation)
        {
            //Todo: 在选择武器的时候更改玩家现在的手持物品类型
            if (index < 0 || index >= _tempTrapBag.Count)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("无效的陷阱选择", player.gameObject));
                return false;
            }

            var trapToPlace = _tempTrapBag[index];
            //bool result = trapToPlace.trapData.placedType switch
            //{
            //    TrapPlacedType.Ground => PlaceGroundTrap(trapToPlace, position, rotation),
            //    TrapPlacedType.Furniture => PlaceFurnitureTrap(player, trapToPlace, position),
            //    _ => false
            //};
            bool result = PlaceGroundTrap(trapToPlace, position, rotation);

            if (!result)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("无法在此放置陷阱", player.gameObject));
                return false;
            }

            trapToPlace.amount--;
            if (trapToPlace.amount <= 0)
            {
                _tempTrapBag.RemoveAt(index);
                SelectedTrapIndex = _tempTrapBag.Count > 0 ? Mathf.Min(SelectedTrapIndex, _tempTrapBag.Count - 1) : 0;
                if (_tempTrapBag.Count == 0)
                {
                    UIHelper.Instance.ShowOneTip(new TipInfo("道具栏已空", player.gameObject));
                }
            }
    
            OnTrapBagUpdated?.Invoke();
            return true;
        }
        
        // 获取道具栏陷阱
        public List<TrapSlotInfo> GetTempTrapSlots()
        {
            return _tempTrapBag;
        }

        #endregion
    }
}