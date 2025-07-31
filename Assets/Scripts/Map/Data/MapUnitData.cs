using System;
using System.Collections.Generic;

namespace KidGame.Core.Data
{
    [Serializable]
    public class MapFurnitureData
    {
        public int serialNumber;//序列号 用于和资源配置列表里的家具做对应

        public FurnitureData furnitureData;

        public List<GridPos> mapPosList;

        public RoomType roomType;

        public float rotation;
    }

    [Serializable]
    public class MapTileData
    {
        public TileData tileData;

        public GridPos mapPos;

        public RoomType roomType;

    }

    [Serializable]
    public class MapWallData
    {
        public WallData wallData;
        public List<GridPos> mapPosList;
        public int stackLayer = 1;
    }
}