using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MapEditorItemGroupConfig",menuName = "KidGameSO/Editor/MapEditorItemGroupConfig")]
public class MapEditorItemGroupConfig : ScriptableObject
{
    [Header("Tile")]
    public List<TileData> TileList;

    [Header("Furniture")]
    public List<FurnitureData> FrunitureList;

    [Header("Blueprint")]
    public List<MapData> MapListList;

}
