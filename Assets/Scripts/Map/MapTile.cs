using KidGame.Core.Data;
using UnityEngine;

namespace KidGame.Core
{
    public class MapTile : MapEntity
    {
        public MapTileData mapTileData;

        public override string itemName { get => mapTileData.tileData.tileName; set { } }
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
