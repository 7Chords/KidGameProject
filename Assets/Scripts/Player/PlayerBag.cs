using System;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.Interface;
using KidGame.UI;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// ������Ʒ����ӿڣ�����ͳһ��ͬ���͵���Ʒ����
    /// </summary>
    public interface BagItemInfoBase
    {
        UseItemType UseItemType { get; }
    }

    /// <summary>
    /// ������λ��Ϣ�ӿ�
    /// </summary>
    public interface ISlotInfo
    {
        BagItemInfoBase ItemData { get; }
        int Amount { get; set; }
    }

    #region ������Ʒ��λ��Ϣ

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
    /// ��ұ��������ࣨ����ģʽ��
    /// </summary>
    public class PlayerBag : Singleton<PlayerBag>
    {
        #region �����ṹ

        public List<ISlotInfo> _tempBag = new();                  // �����������4��
        public List<TrapSlotInfo> _trapBag = new();               // ���屳��
        public List<MaterialSlotInfo> _materialBag = new();       // ���ϱ���
        public List<WeaponSlotInfo> _weaponBag = new();           // ��������

        #endregion

        #region ��ǰѡ�е��������߼�

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
                    _ => "δ֪��Ʒ"
                };

                UIHelper.Instance.ShowOneTip(new TipInfo($"��ѡ��: {itemName}", PlayerController.Instance.gameObject));
                OnTrapBagUpdated?.Invoke();
            }
        }

        public ISlotInfo GetSelectedTempItem()
        {
            return _tempBag.Count > 0 ? _tempBag[_selectedTrapIndex] : null;
        }

        #endregion

        #region �¼�����

        public event Action OnTrapBagUpdated;

        #endregion

        #region ע���뷴ע��

        public void Init()
        {
            PlayerUtil.Instance.RegPlayerPickItem(PlayerGetItem);
        }

        public void Discard()
        {
            PlayerUtil.Instance.UnregPlayerPickItem(PlayerGetItem);
        }

        #endregion

        #region ���������ӿ�

        public List<TrapSlotInfo> GetTrapSlots() => _trapBag;
        public List<MaterialSlotInfo> GetMaterialSlots() => _materialBag;
        public List<ISlotInfo> GetTempSlots() => _tempBag;

        public void TrapBagUpdated() => OnTrapBagUpdated?.Invoke();

        /// <summary>
        /// ���ش浵�еı�������
        /// </summary>
        public void LoadBagData(List<TrapSlotInfo> trapSlots, List<MaterialSlotInfo> materialSlots)
        {
            // _trapBag = trapSlots ?? new List<TrapSlotInfo>();
            // _materialBag = materialSlots ?? new List<MaterialSlotInfo>();
            OnTrapBagUpdated?.Invoke();
        }

        /// <summary>
        /// ���Դӱ�����ɾ��ָ��������Ͳ���
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

        #region ʰȡ��ʹ����Ʒ�߼�

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
                // ������������ת��������
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
                        UIHelper.Instance.ShowOneTip(new TipInfo("�޷�ʶ�����Ʒ����", PlayerController.Instance.gameObject));
                        return;
                }

                UIHelper.Instance.ShowOneTip(new TipInfo("��������������Ʒ�ѷ��뱳��", PlayerController.Instance.gameObject));
            }

            OnTrapBagUpdated?.Invoke();
        }
        
        // �������ӷ������������Ҫ��ô�鷳��
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
        /// ʹ������
        /// </summary>
        public bool UseTrap(ISlotInfo slot, Vector3 position, Quaternion rotation)
        {
            if (slot is TrapSlotInfo trapSlot)
            {
                if (!PlayerController.Instance.GetCanPlaceTrap())
                {
                    UIHelper.Instance.ShowOneTip(new TipInfo("�����޷���������", PlayerController.Instance.gameObject));
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

        #region �������뱳������λ��

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
            // todo.�������ж�����
            _trapBag.Add((TrapSlotInfo)item);

            OnTrapBagUpdated?.Invoke();
        }
        
        #endregion
    }
}
