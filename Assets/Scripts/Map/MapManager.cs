using KidGame.Core.Data;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class MapManager : Singleton<MapManager>
    {
        private MapData _mapData;

        public List<MapTile> mapTileList;
        public Dictionary<RoomType, List<MapTile>> mapTileDic;//搞个房间类型对应的所有瓦片的字典方便外面用
        public List<MapFurniture> mapFurnitureList;
        public Dictionary<RoomType, List<MapFurniture>> mapFurnitureDic;//搞个房间类型对应的所有家具的字典方便外面用
        public List<MapWall> mapWallList;

        public Transform MapGeneratePoint;
        public void Init(MapData mapData)
        {
            _mapData = mapData;
            mapTileList = new List<MapTile>();
            mapTileDic = new Dictionary<RoomType, List<MapTile>>();
            mapFurnitureList = new List<MapFurniture>();
            mapFurnitureDic = new Dictionary<RoomType, List<MapFurniture>>();
            mapWallList = new List<MapWall>();

            GenerateMap(mapData);

        }

        public void GenerateMap(MapData mapData)
        {
            if (mapData == null) return;
            if (MapGeneratePoint == null) return;
            GameObject tmpRoot = GameObject.Find("Map");
            if (tmpRoot != null) DestroyImmediate(tmpRoot);
            GameObject root = new GameObject("Map");
            root.transform.position = MapGeneratePoint.position;
            GameObject tileRoot = new GameObject("Tile");
            GameObject furnitureRoot = new GameObject("Furniture");
            GameObject wallRoot = new GameObject("Wall");
            tileRoot.transform.SetParent(root.transform);
            furnitureRoot.transform.SetParent(root.transform);
            wallRoot.transform.SetParent(root.transform);

            foreach (var tile in mapData.tileList)
            {
                GameObject tileGO = Instantiate(tile.tileData.tilePrefab,
                    new Vector3(tile.mapPos.x, 0, -tile.mapPos.y),
                    Quaternion.identity);
                MapTile mapTile = tileGO.AddComponent<MapTile>();
                mapTile.SetData(tile);
                tileGO.transform.SetParent(tileRoot.transform);
                tileGO.transform.position += root.transform.position;
                mapTileList.Add(mapTile);
                if(mapTileDic.ContainsKey(tile.roomType))
                {
                    mapTileDic[tile.roomType].Add(mapTile);
                }
                else
                {
                    mapTileDic.Add(tile.roomType, new List<MapTile>());
                }
            }
            foreach (var furniture in mapData.furnitureList)
            {
                float averageX = 0, averageZ = 0;
                foreach (var pos in furniture.mapPosList)
                {
                    averageX += pos.x;
                    averageZ += pos.y;
                }
                averageX /= furniture.mapPosList.Count;
                averageZ /= furniture.mapPosList.Count;
                GameObject furnitureGO = Instantiate(furniture.furnitureData.furniturePrefab);
                furnitureGO.transform.position = new Vector3(averageX, 0, -averageZ);
                MapFurniture mapFurniture = furnitureGO.AddComponent<MapFurniture>();
                mapFurniture.SetData(furniture);
                furnitureGO.transform.SetParent(furnitureRoot.transform);
                furnitureGO.transform.position += root.transform.position;
                mapFurnitureList.Add(mapFurniture);
                if (mapFurnitureDic.ContainsKey(furniture.roomType))
                {
                    mapFurnitureDic[furniture.roomType].Add(mapFurniture);
                }
                else
                {
                    mapFurnitureDic.Add(furniture.roomType, new List<MapFurniture>());
                }
            }
            foreach (var wall in mapData.wallList)
            {
                float averageX = 0, averageZ = 0;
                foreach (var pos in wall.mapPosList)
                {
                    averageX += pos.x;
                    averageZ += pos.y;
                }
                averageX /= wall.mapPosList.Count;
                averageZ /= wall.mapPosList.Count;
                //生成堆叠的墙单位
                for (int i = 0; i < wall.stackLayer; i++)
                {
                    GameObject wallGO = Instantiate(wall.wallData.wallPrefab,
                        new Vector3(averageX, 3 * i, -averageZ),
                        Quaternion.identity);
                    wallGO.transform.rotation = wall.wallData.wallPrefab.transform.rotation;
                    MapWall mapWall = wallGO.AddComponent<MapWall>();
                    mapWall.SetData(wall);
                    wallGO.transform.SetParent(wallRoot.transform);
                    wallGO.transform.position += root.transform.position;
                    mapWallList.Add(mapWall);
                }
            }
        }

        public void Discard()
        {
            mapTileList.Clear();
            mapTileList = null;
            mapTileDic.Clear();
            mapTileDic = null;
            mapFurnitureList.Clear();
            mapFurnitureList = null;
            mapFurnitureDic.Clear();
            mapFurnitureDic = null;
            mapWallList.Clear();
            mapWallList = null;
        }
    }
}
