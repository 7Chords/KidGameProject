using System.Collections.Generic;
using System;


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

[Serializable]
public class MapWall
{
    public WallData wallData;
    public GridPos mapPos;
    public int stackLayer = 1;
}