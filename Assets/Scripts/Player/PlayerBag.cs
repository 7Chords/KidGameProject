using System;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.Interface;


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
            }
            else if (iPickable is MaterialBase)
            {
                MaterialData materialData = (iPickable as MaterialBase).materialData;
                if (materialData == null) return;
                MaterialSlotInfo materialSlotInfo = _materialBag.Find(x => x.materialData.materialID == materialData.materialID);
                if (materialSlotInfo == null)
                {
                    _materialBag.Add(new MaterialSlotInfo(materialData,1));
                }
                else
                {
                    materialSlotInfo.amount++;
                }
            }
        }
    }
}