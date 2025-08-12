using KidGame.Interface;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// Ë®²Û
    /// </summary>
    public class Sink : MapFurniture, IInteractive
    {
        public void InteractNegative(GameObject interactor) { }

        public void InteractPositive(GameObject interactor)
        {
            ItemConverter.PlayerBagItemConvertByFurniture(mapFurnitureData.furnitureData.furnitureName);
        }
    }
}
