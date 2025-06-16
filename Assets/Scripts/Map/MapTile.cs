using KidGame.Core.Data;
using UnityEngine;

namespace KidGame.Core
{
    public class MapTile : MonoBehaviour
    {
        public MapTileData mapTileData;

        public MapTile(MapTileData data)
        {
            mapTileData = data;
        }

        public void SetData(MapTileData data)
        {
            mapTileData = data;
        }
    }
}
