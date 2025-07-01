using KidGame.Core.Data;
using UnityEngine;
using KidGame.Interface;
using System.Collections.Generic;
using KidGame.UI;

namespace KidGame.Core
{
    [System.Serializable]
    public class MaterialItem
    {
        public int amount;
        public MaterialData data;

        public MaterialItem(MaterialData data,int amount)
        {
            this.amount = amount;
            this.data = data;
        }
    }


    public class MapFurniture : MonoBehaviour, IPickable
    {
        public MapFurnitureData mapFurnitureData;

        public List<MaterialItem> materialHoldList;

        [SerializeField]
        private List<string> randomPickSfxList;
        public List<string> RandomPickSfxList { get => randomPickSfxList; set { } }

        [SerializeField]
        private GameObject pickPartical;
        public GameObject PickPartical { get => pickPartical; set { } }


        public string itemName { get => mapFurnitureData.furnitureData.furnitureName; set { } }

        private bool canInteract;
        public MapFurniture(MapFurnitureData data)
        {
            mapFurnitureData = data;
        }
        public void SetData(MapFurnitureData data)
        {
            mapFurnitureData = data;
        }

        public void Init(bool canInteract, List<MaterialItem> materialList = null)
        {
            this.canInteract = canInteract;
            materialHoldList = materialList;
            if (canInteract)
            {
                gameObject.layer = LayerMask.NameToLayer("Interactive");
            }
        }

        public void Discard()
        {
            materialHoldList.Clear();
            materialHoldList = null;
        }

        public void InteractNegative()
        {
            if (!canInteract) return;
        }

        public void InteractPositive()
        {
            if (!canInteract) return;
            if (materialHoldList == null || materialHoldList.Count ==0)
            {
                UIHelper.Instance.ShowTip("ø’ø’»Á“≤",gameObject);
            }
            else
            {
                //TODO:
            }
        }

        public void Pick()
        {
            throw new System.NotImplementedException();
        }
    }
}
