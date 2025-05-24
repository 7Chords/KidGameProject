using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "KidGameSO/Map/TileData")]
public class TileData : ScriptableObject
{
    public string tileName;
    public Texture2D texture;
    public GameObject tilePrefab;
}