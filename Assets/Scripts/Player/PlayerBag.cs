using System;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.Interface;
using KidGame.UI;
using UnityEngine;


namespace KidGame.Core
{
    #region 单个物品的信息结构
    
    [Serializable]
    public class TrapSlotInfo
    {
        public TrapData trapData;
        public int amount;

        public TrapSlotInfo(TrapData trapData, int amount)
        {
            this.trapData = trapData;
            this.amount = amount;
        }
    }

    [Serializable]
    public class MaterialSlotInfo
    {
        public MaterialData materialData;
        public int amount;

        public MaterialSlotInfo(MaterialData materialData, int amount)
        {
            this.materialData = materialData;
            this.amount = amount;
        }
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
        public List<MaterialSlotInfo> _materialBag = new List<MaterialSlotInfo>();

        #endregion

        #region 选中

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
                    string trapName = _tempTrapBag[_selectedTrapIndex].trapData.name;
                    UIHelper.Instance.ShowOneTip(new TipInfo($"已选择: {trapName}", PlayerController.Instance.gameObject));
                }
                OnTrapBagUpdated?.Invoke();
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
            _trapBag = trapSlots ?? new List<TrapSlotInfo>();
            _materialBag = materialSlots ?? new List<MaterialSlotInfo>();

            OnTrapBagUpdated?.Invoke();
        }
        
        public bool TryDelItemInBag(List<TrapSlotInfo> requireTrapSlots = null, List<MaterialSlotInfo> requireMaterialSlots = null)
        {
            if(requireTrapSlots == null&&requireMaterialSlots==null) return true;
            if(requireTrapSlots != null)
            {
                foreach (var trapSlot in requireTrapSlots)
                {
                    var existingSlot = _trapBag.Find(x => x.trapData.trapID == trapSlot.trapData.trapID);
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
                    var existingSlot = _materialBag.Find(x => x.materialData.materialID == materialSlot.materialData.materialID);
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
                    TrapSlotInfo trapSlotInfo = _trapBag.Find(x => x.trapData.trapID == trapData.trapID);
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
                    _materialBag.Find(x => x.materialData.materialID == materialData.materialID);
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
            GameObject newTrap = TrapFactory.Create(trapToPlace.trapData, position);
            if (newTrap != null)
            {
                newTrap.transform.rotation = rotation;
                return true;
            }

            return false;
        }

        private bool PlaceFurnitureTrap(PlayerController player, TrapSlotInfo trapToPlace, Vector3 position)
        {
            var furniture = player.GetClosestPickable() as MapFurniture;
            if (furniture == null) return false;

            GameObject newTrap = TrapFactory.Create(trapToPlace.trapData, position);
            if (newTrap != null)
            {
                furniture.SetTrap(newTrap);
                return true;
            }

            return false;
        }

        #endregion
        
        #region 道具栏方法
        
        public bool TryAddToTempTrapBag(TrapData trapData)
        {
            var existingSlot = _tempTrapBag.Find(x => x.trapData.trapID == trapData.trapID);

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
            if (index < 0 || index >= _tempTrapBag.Count)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("无效的陷阱选择", player.gameObject));
                return false;
            }

            var trapToPlace = _tempTrapBag[index];
            bool result = trapToPlace.trapData.placedType switch
            {
                TrapPlacedType.Ground => PlaceGroundTrap(trapToPlace, position, rotation),
                TrapPlacedType.Furniture => PlaceFurnitureTrap(player, trapToPlace, position),
                _ => false
            };

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