using System.Collections.Generic;
using System;

//public enum MapGridState
//{
//    Tile,//��ש
//    Furniture,//��ש�Ϸ��˼Ҿ�
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