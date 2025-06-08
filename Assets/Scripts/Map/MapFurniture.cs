using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFurniture : MonoBehaviour
{
    public MapFurnitureData mapFurnitureData;

    public MapFurniture(MapFurnitureData data)
    {
        mapFurnitureData = data;
    }

    public void SetData(MapFurnitureData data)
    {
        mapFurnitureData = data;
    }
}
