using KidGame.Core.Data;
using UnityEngine;

namespace KidGame.Core
{
    public class MapWall : MapEntity
    {
        public MapWallData mapWallData;

        public override string itemName { get => mapWallData.wallData.wallName; set { } }

        public MapWall(MapWallData data)
        {
            mapWallData = data;
        }
        public void SetData(MapWallData data)
        {
            mapWallData = data;
        }
    }
}
