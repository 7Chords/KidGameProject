using UnityEngine;

namespace KidGame.Core.Data
{
    [CreateAssetMenu(fileName = "TileData", menuName = "KidGameSO/Map/TileData")]
    public class TileData : ScriptableObject
    {
        public string tileName;
        public Texture2D texture;
        public GameObject tilePrefab;
    }
}