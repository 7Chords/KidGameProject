using System.Collections.Generic;
using System;


[Serializable]
public class MapFurnitureData
{
    public FurnitureData furnitureData;

    public List<GridPos> mapPosList;
}

[Serializable]
public class MapTileData
{
    public TileData tileData;

    public GridPos mapPos;
}

[Serializable]
public class MapWallData
{
    public WallData wallData;
    public GridPos mapPos;
    public int stackLayer = 1;
}