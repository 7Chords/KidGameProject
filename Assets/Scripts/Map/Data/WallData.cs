using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core.Data
{
    [CreateAssetMenu(fileName = "WallData", menuName = "KidGameSO/Map/WallData")]
    public class WallData : ScriptableObject
    {
        public string wallName;
        public Texture2D texture;
        public List<GridPos> posList;
        public GameObject wallPrefab;
    }
}