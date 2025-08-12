using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public static class ItemConverter
    {
        public static void PlayerBagItemConvertByFurniture(string furnitureName)
        {
            ISlotInfo slotInfo = PlayerBag.Instance.GetSelectedQuickAccessItem();
            if (slotInfo == null) return;
            if(slotInfo is TrapSlotInfo)
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
                case "Ë®²Û":
                    switch(slotInfo.materialData.materialName)
                    {
                        case "Í°":
                            PlayerBag.Instance.DeleteItemInCombineBag(slotInfo.materialData.id, 1);
                            PlayerBag.Instance.AddItemToCombineBag("M003", UseItemType.Material, 1);
                            break;
                    }
                    break;
            }
        }
    }
}
