using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapWall : MonoBehaviour
{
    public MapWallData mapWallData;

    public MapWall(MapWallData data)
    {
        mapWallData = data;
    }
    public void SetData(MapWallData data)
    {
        mapWallData = data;
    }
}
