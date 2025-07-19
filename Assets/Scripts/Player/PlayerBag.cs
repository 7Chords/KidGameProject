using System;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.Interface;
using KidGame.UI;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// ������Ʒ����
    /// </summary>
    
    public interface BagItemInfoBase
    {
        UseItemType UseItemType { get; }
    }


    #region ������Ʒ����Ϣ�ṹ
    
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
        #region ��Ʒ�б�

        // ��ǰ�񵽵���Ʒ���Լ��Ӵ浵�ж�ȡ���ݡ��������ݵ��߼�
        // ��������������Ϊ4�����ߣ�ʰȡ���ĵ����ȷ���������������ŷ��뱳��
        public List<TrapSlotInfo> _tempTrapBag = new List<TrapSlotInfo>();

        // ���屳��
        public List<TrapSlotInfo> _trapBag = new List<TrapSlotInfo>();
        // ���ϱ���
        public List<MaterialSlotInfo> _materialBag = new List<MaterialSlotInfo>();
        // �ֱֳ���
        public List<WeaponSlotInfo>  _weaponBag = new List<WeaponSlotInfo>();

        #endregion

        #region ѡ��

        public event Action<TrapData> SelectTrapAction;

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
                    string trapName = _tempTrapBag[_selectedTrapIndex].trapData.trapName;
                    UIHelper.Instance.ShowOneTip(new TipInfo($"��ѡ��: {trapName}", PlayerController.Instance.gameObject));
                }
                OnTrapBagUpdated?.Invoke();
                SelectTrapAction?.Invoke(_tempTrapBag[_selectedTrapIndex].trapData);
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
                UIHelper.Instance.ShowOneTip(new TipInfo("�����޷���������", PlayerController.Instance.gameObject));
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

        //ʹ�õĵ���Ϊ�ֳֵ��ߣ�
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
        
        #region ����������
        
        public bool TryAddToTempTrapBag(TrapData trapData)
        {
            var existingSlot = _tempTrapBag.Find(x => x.trapData.id == trapData.id);

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
            //Todo: ��ѡ��������ʱ�����������ڵ��ֳ���Ʒ����
            if (index < 0 || index >= _tempTrapBag.Count)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("��Ч������ѡ��", player.gameObject));
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