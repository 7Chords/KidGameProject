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

        public MapFurniture(MapFurnitureData data)
        {
            mapFurnitureData = data;
        }
        public void SetData(MapFurnitureData data)
        {
            mapFurnitureData = data;
        }

        public void Init()
        {

        }

        public void Discard()
        {

        }

        public void InteractNegative()
        {
            
        }

        public void InteractPositive()
        {
            
        }


    }
}
