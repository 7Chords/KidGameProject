using System.Collections;
using System.Collections.Generic;
using KidGame.Core.Data;
using UnityEngine;

namespace KidGame.Editor
{
    [CreateAssetMenu(fileName = "MapEditorItemGroupConfig", menuName = "KidGameSO/Editor/MapEditorItemGroupConfig")]
    public class MapEditorItemGroupConfig : ScriptableObject
    {
        [Header("Tile")] public List<TileData> TileList;

        [Header("Furniture")] public List<FurnitureData> FrunitureList;

        [Header("Wall")] public List<WallData> WallList;

        [Header("Blueprint")] public List<MapData> MapListList;

        [Header("Other")]
        public Sprite FurntureRoomTypeNoMatchWarningSprite;
    }
}