using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MapData", menuName = "KidGameSO/Map/MapData")]
public class MapData : ScriptableObject
{
    public string MapName;
    public List<MapTileData> tileList = new List<MapTileData>();
    public List<MapWallData> wallList = new List<MapWallData>();
    public List<MapFurnitureData> furnitureList = new List<MapFurnitureData>();
}
