using System;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.Interface;
using KidGame.UI;
using UnityEngine;


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

        #region ѡ��

        private int _selectedTrapIndex = 0; // ��ǰѡ�е���������

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
        
        #region �����¼�

        public event Action OnTrapBagUpdated;

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
            
            OnTrapBagUpdated?.Invoke();
        }
        
            
        // ��ȡ��ǰѡ�е�����
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
                UIHelper.Instance.ShowTip("û�п��õ����壡", player.gameObject);
                return false;
            }

            // ����������ͺͷ�������
            if (trapToPlace.trapData.placedType == TrapPlacedType.Ground)
            {
                return PlaceGroundTrap(trapToPlace, position, rotation);
            }
            else if (trapToPlace.trapData.placedType == TrapPlacedType.Furniture)
            {
                return PlaceFurnitureTrap(player, trapToPlace, position);
            }

            UIHelper.Instance.ShowTip("�������������ڼҾ��ϣ�", player.gameObject);
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
                    // �Զ�����ѡ������
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