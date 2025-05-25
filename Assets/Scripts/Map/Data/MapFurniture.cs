using System.Collections.Generic;
using System;

//public enum MapGridState
//{
//    Tile,//地砖
//    Furniture,//地砖上放了家具
//}

[Serializable]
public class MapFurniture
{
    public FurnitureData furnitureData;

    public List<GridPos> mapPosList;
}

[Serializable]
public class MapTile
{
    public TileData tileData;

    public GridPos mapPos;
}