using KidGame.UI;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{

    /// <summary>
    /// ������Ʒ����ӿڣ�����ͳһ��ͬ���͵���Ʒ����
    /// </summary>
    public interface BagItemInfoBase
    {
        string Id { get; }
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

        public List<ISlotInfo> QuickAccessBag;//�����������4��
        public List<ISlotInfo> BackBag;//��汳��

        //TIPS:����ڷ������п���combineBag������ �������������б���ȥ����
        //����Ҫɾ��ĳ��id�ĵ��߾���Ҫ������ȥ�� �����ȼӵ������ټӱ���

        #endregion





        #region ��ǰѡ�е��������߼�


        private int _selectedIndex = 0;

        //SelectedIndex��Զֻ�ᱻ�����ֽ������� ��GamePlayPanelController�н�������
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {

                int newIndex = Mathf.Clamp(value, 0, GlobalValue.QUICK_ACCESS_BAG_CAPACITY);

                _selectedIndex = newIndex;

                if(_selectedIndex < QuickAccessBag.Count)
                {
                    var selectedSlot = QuickAccessBag[_selectedIndex];
                    if (selectedSlot != null) MsgCenter.SendMsg(MsgConst.ON_SELECT_ITEM, selectedSlot);

                    string itemName = selectedSlot.ItemData switch
                    {
                        TrapData trap => trap.trapName,
                        WeaponData weapon => weapon.name,
                        FoodData food => food.foodName,
                        MaterialData mat => mat.materialName,
                        _ => "δ֪��Ʒ"
                    };

                    UIHelper.Instance.ShowOneTip(new TipInfo($"��ѡ��: {itemName}", PlayerController.Instance.transform.position));
                }
                else
                {
                    MsgCenter.SendMsg(MsgConst.ON_SELECT_ITEM, null);
                }
                MsgCenter.SendMsgAct(MsgConst.ON_QUICK_BAG_UPDATE);
            }
        }

        public ISlotInfo GetSelectedQuickAccessItem()
        {
            return _selectedIndex < QuickAccessBag.Count ? QuickAccessBag[_selectedIndex] : null;
        }

        #endregion


        #region ע���뷴ע��

        public void Init()
        {
            MsgCenter.RegisterMsg(MsgConst.ON_PICK_ITEM, PlayerGetOneItem);
            QuickAccessBag = new List<ISlotInfo>(GlobalValue.QUICK_ACCESS_BAG_CAPACITY);
            BackBag = new List<ISlotInfo>();
        }

        public void Discard()
        {
            MsgCenter.UnregisterMsg(MsgConst.ON_PICK_ITEM, PlayerGetOneItem);
        }

        #endregion

        #region ���������ӿ�
        public List<ISlotInfo> GetQuickAccessBag() => QuickAccessBag;

        /// <summary>
        /// ���ش浵�еı������� todo:guihuala
        /// </summary>
        public void LoadBagData(List<TrapSlotInfo> trapSlots, List<MaterialSlotInfo> materialSlots)
        {
            // _trapBag = trapSlots ?? new List<TrapSlotInfo>();
            // _materialBag = materialSlots ?? new List<MaterialSlotInfo>();
            MsgCenter.SendMsgAct(MsgConst.ON_QUICK_BAG_UPDATE);
        }




        /// <summary>
        /// ���Դӱ���(�������͵�����)��ɾ��ָ�������ĵ���
        /// </summary>
        /// <param name="itemId">����id</param>
        /// <param name="delAmount">Ҫɾ��������</param>
        /// <returns>�Ƿ��Ƴ��ɹ�</returns>
        public bool DeleteItemInCombineBag(string itemId, int delAmount)
        {
            if (string.IsNullOrEmpty(itemId) || delAmount <= 0) return false;
            bool slotInBackBag = true;
            ISlotInfo slotInfo = BackBag.Find(X => X.ItemData.Id == itemId);
            if (slotInfo == null)
            {
                slotInBackBag = false;
                slotInfo = QuickAccessBag.Find(x => x.ItemData.Id == itemId);
            }
            if (slotInfo == null) return false;
            slotInfo.Amount = Mathf.Max(0, slotInfo.Amount - delAmount);
            if(slotInfo.Amount == 0)
            {
                if (slotInBackBag) BackBag.Remove(slotInfo);
                else 
                {
                    QuickAccessBag.Remove(slotInfo);
                    if(SelectedIndex >= QuickAccessBag.Count)
                    {
                        MsgCenter.SendMsg(MsgConst.ON_SELECT_ITEM, null);
                    }
                }
            }
            MsgCenter.SendMsgAct(MsgConst.ON_QUICK_BAG_UPDATE);
            return true;
        }

        /// <summary>
        /// ��鱳���Ƿ���һ��������ĳ����Ʒ
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="delAmount"></param>
        /// <returns></returns>
        public bool CheckItemEnoughInCombineBag(string itemId, int delAmount)
        {
            if (string.IsNullOrEmpty(itemId) || delAmount <= 0) return false;
            ISlotInfo slotInfo = BackBag.Find(X => X.ItemData.Id == itemId);
            if (slotInfo == null)
            {
                slotInfo = QuickAccessBag.Find(X => X.ItemData.Id == itemId);
            }
            if (slotInfo == null) return false;
            if (slotInfo.Amount < delAmount) return false;
            return true;
        }

        /// <summary>
        /// �����Ʒ���������ȼӵ������� ���˲ŵ�������
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="addAmount"></param>
        public bool AddItemToCombineBag(string itemId, UseItemType itemType, int addAmount)
        {
            var existing = QuickAccessBag.Find(x => x.ItemData.Id == itemId);
            if(existing == null)
            {
                existing = BackBag.Find(x => x.ItemData.Id == itemId);
            }
            if (existing != null)
            {
                existing.Amount += addAmount;
            }
            else if (QuickAccessBag.Count < GlobalValue.QUICK_ACCESS_BAG_CAPACITY)
            {
                switch (itemType)
                {
                    case UseItemType.trap:
                        QuickAccessBag.Add(new TrapSlotInfo(SoLoader.Instance.GetTrapDataById(itemId), addAmount));
                        break;
                    case UseItemType.Material:
                        QuickAccessBag.Add(new MaterialSlotInfo(SoLoader.Instance.GetMaterialDataDataById(itemId), addAmount));
                        break;
                    case UseItemType.weapon:
                        QuickAccessBag.Add(new WeaponSlotInfo(SoLoader.Instance.GetWeaponDataById(itemId), addAmount));
                        break;
                    //todo:food
                    default:
                        break;
                }
                //��ǰѡ�е���Ʒ�����޵��� ����ˢ��
                if(QuickAccessBag.Count == SelectedIndex + 1)
                {
                    MsgCenter.SendMsg(MsgConst.ON_SELECT_ITEM, QuickAccessBag[SelectedIndex]);
                }
            }
            else if(BackBag.Count<GlobalValue.BACKPACK_CAPACITY)
            {
                // ������������ת��������
                switch (itemType)
                {
                    case UseItemType.trap:
                        BackBag.Add(new TrapSlotInfo(SoLoader.Instance.GetTrapDataById(itemId), 1));
                        break;
                    case UseItemType.Material:
                        BackBag.Add(new MaterialSlotInfo(SoLoader.Instance.GetMaterialDataDataById(itemId), 1));
                        break;
                    case UseItemType.weapon:
                        BackBag.Add(new WeaponSlotInfo(SoLoader.Instance.GetWeaponDataById(itemId), 1));
                        break;
                    //todo:food
                    default:
                        break;
                }
            }
            else
            {
                return false;
            }

            MsgCenter.SendMsgAct(MsgConst.ON_QUICK_BAG_UPDATE);
            return true;
        }

        #endregion

        #region ʰȡ��ʹ����Ʒ�߼�

        private void PlayerGetOneItem(params object[] objs)
        {
            if (objs == null || objs.Length == 0) return;

            string itemId = objs[0] as string;
            UseItemType itemType = (UseItemType)objs[1];

            if (string.IsNullOrEmpty(itemId)) return;

            switch (itemType)
            {
                case UseItemType.trap:
                    AddItemToCombineBag(itemId, itemType,1);
                    break;
                case UseItemType.Material:
                    AddItemToCombineBag(itemId, itemType,1);
                    break;
                case UseItemType.weapon:
                    AddItemToCombineBag(itemId, itemType,1);
                    break;
                //todo:food
                default:
                    break;
            }
        }

        /// <summary>
        /// ʹ������
        /// </summary>
        //public bool UseTrap(ISlotInfo slot, Vector3 position, Quaternion rotation)
        //{
        //    if (!PlayerController.Instance.GetCanPlaceTrap())
        //    {
        //        UIHelper.Instance.ShowOneTip(new TipInfo("�����޷���������", PlayerController.Instance.gameObject));
        //        return false;
        //    }
        //    return true;
        //}

        public bool UseWeapon(ISlotInfo slot, Vector3 position, Quaternion rotation)
        {
            if (slot is WeaponSlotInfo weaponSlot)
            {
                // �������ڵ��ֳ����� ���Ұ�player����Ǹ�����ָ���
                PlayerController.Instance.DiscardWeapon();

                GameObject newWeapon = WeaponFactory.CreateEntity(weaponSlot.weaponData, position, this.gameObject.transform);
                WeaponBase curWeaponScript = newWeapon.GetComponent<WeaponBase>();
                // �����Զ�̵���������Ʒ:
                if (newWeapon != null
                    && curWeaponScript.weaponData.weaponType == 1
                    && curWeaponScript.weaponData.useType == 0)
                {
                    // �������� ������ѡ����Ʒ��ʱ�� ��������ɵ�����ֱ�ӿ�ʼ��Ҫ�߼�
                    newWeapon.transform.SetParent(null);
                    newWeapon.transform.rotation = rotation;
                    DeleteItemInCombineBag(slot.ItemData.Id, 1);
                    curWeaponScript.SetOnHandOrNot(false);
                    return true;
                }
            }
            return false;
        }
        public bool UseFood(ISlotInfo slot, PlayerController player) => false;
        public bool UseMaterial(ISlotInfo slot, PlayerController player) => false;

        #endregion
       



        #region �������뱳������λ��

        public void MoveItemToQuickAccessBag(int selectIndex)
        {
            if (selectIndex < 0 || selectIndex >= BackBag.Count || QuickAccessBag.Count >= GlobalValue.QUICK_ACCESS_BAG_CAPACITY)
                return;

            var item = BackBag[selectIndex];
            BackBag.RemoveAt(selectIndex);
            QuickAccessBag.Add(item);
            MsgCenter.SendMsgAct(MsgConst.ON_QUICK_BAG_UPDATE);

        }

        public void MoveItemToBackBag(int selectIndex)
        {
            if (selectIndex < 0 || selectIndex >= QuickAccessBag.Count || BackBag.Count >= GlobalValue.BACKPACK_CAPACITY)
                return;
            
            var item = QuickAccessBag[selectIndex];
            QuickAccessBag.RemoveAt(selectIndex);
            BackBag.Add(item);
            MsgCenter.SendMsgAct(MsgConst.ON_QUICK_BAG_UPDATE);

        }

        #endregion

        #region ��������Ҿ���������λ��

        public void MoveItemToItemContainer(int selectIndex,PopItemContainerProp prop)
        {
            if (selectIndex < 0 || selectIndex >= BackBag.Count || prop.items.Count >= prop.maxCount)
                return;
            
            var item = BackBag[selectIndex];
            if(!(item.ItemData is MaterialData))return;
            BackBag.RemoveAt(selectIndex);
            prop.originItems.Add(new MaterialItem(item.ItemData as MaterialData,item.Amount));

            //OnQuickAccessBagUpdated?.Invoke();
        }

        public void MoveContainerItemToBackBag(int selectIndex,PopItemContainerProp prop)
        {
            if (selectIndex < 0 || selectIndex >= prop.maxCount || BackBag.Count >= GlobalValue.BACKPACK_CAPACITY)
                return;
            
            var item = prop.items[selectIndex];
            if(!(item.ItemData is MaterialData))return;
            AddItemToCombineBag(item.ItemData.Id, item.ItemData.UseItemType, item.Amount);
            //BackBag.Add(item);
            prop.originItems.RemoveAt(selectIndex);
            prop.items.RemoveAt(selectIndex);
            

            //OnQuickAccessBagUpdated?.Invoke();
        }
        
        #endregion
    }
}
