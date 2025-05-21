using System.Collections.Generic;
using System;
public enum MapGridState
{
    Tile,//��ש
    Furniture,//��ש�Ϸ��˼Ҿ�
}

[Serializable]
public class MapItem
{
    public MapGridState gridState;

    public TileData tileData;

    public FurnitureData furnitureData;

    public List<GridPos> mapPosList;
}