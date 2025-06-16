using KidGame.Core.Data;
using UnityEngine;

namespace KidGame.Core
{
    public class MapFurniture : MonoBehaviour
    {
        public MapFurnitureData mapFurnitureData;

        public MapFurniture(MapFurnitureData data)
        {
            mapFurnitureData = data;
        }

        public void SetData(MapFurnitureData data)
        {
            mapFurnitureData = data;
        }
    }
}
