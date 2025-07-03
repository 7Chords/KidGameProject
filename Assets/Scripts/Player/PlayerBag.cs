using System;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.Interface;


namespace KidGame.Core
{
    #region ������Ʒ����Ϣ�ṹ
    
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
        #region ��Ʒ�б�

        // ��ǰ�񵽵���Ʒ���Լ��Ӵ浵�ж�ȡ���ݡ��������ݵ��߼�
        public List<TrapSlotInfo> _trapBag = new List<TrapSlotInfo>();
        public List<MaterialSlotInfo> _materialBag = new List<MaterialSlotInfo>();

        #endregion

        #region ע���¼�

        public void Init()
        {
            PlayerUtil.Instance.RegPlayerPickItem(PlayerPickItem);
        }

        public void Discard()
        {
            PlayerUtil.Instance.UnregPlayerPickItem(PlayerPickItem);
        }

        #endregion

        #region �ⲿ���÷���

        public List<TrapSlotInfo> GetTrapSlots()
        {
            return _trapBag;
        }

        public List<MaterialSlotInfo> GetMaterialSlots()
        {
            return _materialBag;
        }
        
        // ���ر�������
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