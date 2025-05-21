using System.Collections.Generic;
using System;
public enum MapGridState
{
    Tile,//地砖
    Furniture,//地砖上放了家具
}

[Serializable]
public class MapItem
{
    public MapGridState gridState;

    public TileData tileData;

    public FurnitureData furnitureData;

    public List<GridPos> mapPosList;
}