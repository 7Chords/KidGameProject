using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public MapTileData mapTileData;

    public MapTile(MapTileData data)
    {
        mapTileData = data;
    }

    public void SetData(MapTileData data)
    {
        mapTileData = data;
    }
}
