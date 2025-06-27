using KidGame.Core.Data;
using UnityEngine;
using KidGame.Interface;
using System.Collections.Generic;

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


    public class MapFurniture : MonoBehaviour,IInteractive
    {
        public MapFurnitureData mapFurnitureData;

        public List<MaterialItem> materialHoldList;

        [SerializeField]
        private List<string> randomInteractSfxList;
        public List<string> RandomInteractSfxList { get => randomInteractSfxList; set { randomInteractSfxList = value; } }

        [SerializeField]
        private ParticleSystem interactPartical;
        public ParticleSystem InteractPartical { get => interactPartical; set { interactPartical = value; } }

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
            if(canInteract)
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
            Debug.Log("和" + mapFurnitureData.furnitureData.furnitureName + "互动了！可以获得里面的材料！");
        }


    }
}
