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
    }


    public class MapFurniture : MonoBehaviour,IInteractive
    {
        public MapFurnitureData mapFurnitureData;

        public List<MaterialItem> materialHoldList;

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
        }

        public void Discard()
        {

        }

        public void InteractNegative()
        {
            if (!canInteract) return;
        }

        public void InteractPositive()
        {
            if (!canInteract) return;
        }


    }
}
