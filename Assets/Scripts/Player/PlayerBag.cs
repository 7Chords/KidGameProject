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
                if (_trapBag.Count > 0)
                {
                    _selectedTrapIndex = Mathf.Clamp(value, 0, _trapBag.Count - 1);
                    OnTrapBagUpdated?.Invoke();
                }
                else
                {
                    _selectedTrapIndex = 0;
                }
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
        
        // 加载背包数据
        public void LoadBagData(List<TrapSlotInfo> trapSlots, List<MaterialSlotInfo> materialSlots)
        {
            _trapBag = trapSlots ?? new List<TrapSlotInfo>();
            _materialBag = materialSlots ?? new List<MaterialSlotInfo>();
            
            OnTrapBagUpdated?.Invoke();
        }
        
            
        // 获取当前选中的陷阱
        public TrapSlotInfo GetSelectedTrap()
        {
            if (_trapBag.Count == 0) return null;
            return _trapBag[SelectedTrapIndex];
        }
        
        #endregion
        
        
        private void PlayerPickItem(IPickable iPickable)
        {
            if (iPickable == null) return;
            if (iPickable is TrapBase)
            {
                TrapData trapData = (iPickable as TrapBase).TrapData;
                if (trapData == null) return;
                TrapSlotInfo trapSlotInfo = _trapBag.Find(x => x.trapData.trapID == trapData.trapID);
                if (trapSlotInfo == null)
                {
                    _trapBag.Add(new TrapSlotInfo(trapData, 1));
                }
                else
                {
                    trapSlotInfo.amount++;
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
        
        public bool TryUseSelectedTrap(PlayerController player, Vector3 position, Quaternion rotation)
        {
            var trapToPlace = GetSelectedTrap();
            if (trapToPlace == null)
            {
                UIHelper.Instance.ShowTip("没有可用的陷阱！", player.gameObject);
                return false;
            }

            // 检查陷阱类型和放置条件
            if (trapToPlace.trapData.placedType == TrapPlacedType.Ground)
            {
                return PlaceGroundTrap(trapToPlace, position, rotation);
            }
            else if (trapToPlace.trapData.placedType == TrapPlacedType.Furniture)
            {
                return PlaceFurnitureTrap(player, trapToPlace, position);
            }

            UIHelper.Instance.ShowTip("该陷阱必须放置在家具上！", player.gameObject);
            return false;
        }

        private bool PlaceGroundTrap(TrapSlotInfo trapToPlace, Vector3 position, Quaternion rotation)
        {
            GameObject newTrap = TrapFactory.Create(trapToPlace.trapData, position);
            if (newTrap != null)
            {
                newTrap.transform.rotation = rotation;
                ReduceTrapAmount(trapToPlace);
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
                ReduceTrapAmount(trapToPlace);
                return true;
            }

            return false;
        }

        private void ReduceTrapAmount(TrapSlotInfo trapToPlace)
        {
            if (--trapToPlace.amount <= 0)
            {
                int index = _trapBag.IndexOf(trapToPlace);
                if (index >= 0)
                {
                    _trapBag.RemoveAt(index);
                    // 自动调整选中索引
                    if (SelectedTrapIndex >= _trapBag.Count && _trapBag.Count > 0)
                    {
                        SelectedTrapIndex = _trapBag.Count - 1;
                    }
                    else if (_trapBag.Count == 0)
                    {
                        SelectedTrapIndex = 0;
                    }
                }
            }

            OnTrapBagUpdated?.Invoke();
        }
    }
}