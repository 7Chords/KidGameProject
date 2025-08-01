using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core.Data
{
    [System.Serializable]
    public struct GridPos
    {
        public int x, y;

        public GridPos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [CreateAssetMenu(fileName = "FurnitureData", menuName = "KidGameSO/Map/FurnitureData")]
    public class FurnitureData : ScriptableObject
    {
        public string furnitureName;
        public Texture2D texture;
        public List<GridPos> posList;
        public GameObject furniturePrefab;
        public Vector2Int gridLayout;
    }
}