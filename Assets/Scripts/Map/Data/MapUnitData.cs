using System;
using System.Collections.Generic;

namespace KidGame.Core.Data
{
    [Serializable]
    public class MapFurnitureData
    {
        public int serialNumber;//���к� ���ں���Դ�����б���ļҾ�����Ӧ

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