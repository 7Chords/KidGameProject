using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 物品转换类
    /// </summary>
    public static class ItemConverter
    {

        /// <summary>
        /// 通过家具转化玩家的背包道具
        /// </summary>
        /// <param name="furnitureName"></param>
        public static void PlayerBagItemConvertByFurniture(string furnitureName)
        {
            ISlotInfo slotInfo = PlayerBag.Instance.GetSelectedQuickAccessItem();
            if (slotInfo == null)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("似乎啥也没有发生", PlayerController.Instance.transform.position));
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
                case "水槽":
                    switch(slotInfo.materialData.materialName)
                    {
                        case "桶":
                            PlayerBag.Instance.DeleteItemInCombineBag(slotInfo.materialData.id, 1);
                            PlayerBag.Instance.AddItemToCombineBag("M003", UseItemType.Material, 1);
                            UIHelper.Instance.ShowOneTip(new TipInfo("获得了装满水的桶×1", PlayerController.Instance.transform.position));
                            break;
                        default:
                            UIHelper.Instance.ShowOneTip(new TipInfo("似乎啥也没有发生", PlayerController.Instance.transform.position));
                            break;
                    }
                    break;
            }
        }
    }
}
