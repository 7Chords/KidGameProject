using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MapData", menuName = "KidGameSO/Map/MapData")]
public class MapData : ScriptableObject
{
    public string MapName;
    public List<MapTile> tileList = new List<MapTile>();
    public List<MapWall> wallList = new List<MapWall>();
    public List<MapFurniture> furnitureList = new List<MapFurniture>();
}
