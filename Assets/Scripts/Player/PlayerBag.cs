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
        // ��������������Ϊ4�����ߣ�ʰȡ���ĵ����ȷ���������������ŷ��뱳��
        public List<TrapSlotInfo> _tempTrapBag = new List<TrapSlotInfo>();

        // ���屳��
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
                int newIndex = _tempTrapBag.Count > 0 ? Mathf.Clamp(value, 0, _tempTrapBag.Count - 1) : 0;
        
                if (_selectedTrapIndex == newIndex) return;
        
                _selectedTrapIndex = newIndex;
                if (_tempTrapBag.Count > _selectedTrapIndex)
                {
                    string trapName = _tempTrapBag[_selectedTrapIndex].trapData.name;
                    UIHelper.Instance.ShowOneTip(new TipInfo($"��ѡ��: {trapName}", PlayerController.Instance.gameObject));
                }
                OnTrapBagUpdated?.Invoke();
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

        public void TrapBagUpdated()
        {
            OnTrapBagUpdated?.Invoke();
        }

        // ���ر�������
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
        

        // ��ȡ��ǰѡ�е�����
        public TrapSlotInfo GetSelectedTrap()
        {
            if (_trapBag.Count == 0) return null;
            return _trapBag[SelectedTrapIndex];
        }

        #endregion

        #region ��������

        private void PlayerPickItem(IPickable iPickable)
        {
            if (iPickable == null) return;
        
            if (iPickable is TrapBase)
            {
                TrapData trapData = (iPickable as TrapBase).TrapData;
                if (trapData == null) return;
                
                if (!TryAddToTempTrapBag(trapData))
                {
                    // ���������������뱳��
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
        
        #region ����������
        
        public bool TryAddToTempTrapBag(TrapData trapData)
        {
            var existingSlot = _tempTrapBag.Find(x => x.trapData.trapID == trapData.trapID);

            if (existingSlot != null)
            {
                existingSlot.amount++;
                OnTrapBagUpdated?.Invoke();
                return true;
            }

            // ���û����ͬ�����һ��п�λ
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
                UIHelper.Instance.ShowOneTip(new TipInfo("��Ч������ѡ��", player.gameObject));
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
                UIHelper.Instance.ShowOneTip(new TipInfo("�޷��ڴ˷�������", player.gameObject));
                return false;
            }

            trapToPlace.amount--;
            if (trapToPlace.amount <= 0)
            {
                _tempTrapBag.RemoveAt(index);
                SelectedTrapIndex = _tempTrapBag.Count > 0 ? Mathf.Min(SelectedTrapIndex, _tempTrapBag.Count - 1) : 0;
                if (_tempTrapBag.Count == 0)
                {
                    UIHelper.Instance.ShowOneTip(new TipInfo("�������ѿ�", player.gameObject));
                }
            }
    
            OnTrapBagUpdated?.Invoke();
            return true;
        }
        
        // ��ȡ����������
        public List<TrapSlotInfo> GetTempTrapSlots()
        {
            return _tempTrapBag;
        }

        #endregion
    }
}