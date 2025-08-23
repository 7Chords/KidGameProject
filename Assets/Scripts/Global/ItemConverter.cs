using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ��Ʒת����
    /// </summary>
    public static class ItemConverter
    {

        /// <summary>
        /// ͨ���Ҿ�ת����ҵı�������
        /// </summary>
        /// <param name="furnitureName"></param>
        public static void PlayerBagItemConvertByFurniture(string furnitureName)
        {
            ISlotInfo slotInfo = PlayerBag.Instance.GetSelectedQuickAccessItem();
            if (slotInfo == null)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("�ƺ�ɶҲû�з���", PlayerController.Instance.transform.position));
                return;
            }
            if (slotInfo is TrapSlotInfo)
            {

            }
            else if (slotInfo is MaterialSlotInfo)
            {
                PlayerBagMaterialConvertByFurniture(furnitureName, slotInfo as MaterialSlotInfo);
            }
            else if (slotInfo is WeaponSlotInfo)
            {

            }
            //else if (slotInfo is FoodSlotInfo)
            //{

            //}
        }

        private static void PlayerBagMaterialConvertByFurniture(string furnitureName, MaterialSlotInfo slotInfo)
        {
            switch(furnitureName)
            {
                case "ˮ��":
                    switch(slotInfo.materialData.materialName)
                    {
                        case "Ͱ":
                            PlayerBag.Instance.DeleteItemInCombineBag(slotInfo.materialData.id, 1);
                            PlayerBag.Instance.AddItemToCombineBag("M003", UseItemType.Material, 1);
                            UIHelper.Instance.ShowOneTip(new TipInfo("�����װ��ˮ��Ͱ��1", PlayerController.Instance.transform.position));
                            break;
                        default:
                            UIHelper.Instance.ShowOneTip(new TipInfo("�ƺ�ɶҲû�з���", PlayerController.Instance.transform.position));
                            break;
                    }
                    break;
            }
        }
    }
}
