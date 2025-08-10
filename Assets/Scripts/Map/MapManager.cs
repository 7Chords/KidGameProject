using KidGame.Core.Data;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace KidGame.Core
{
    public class RoomInfo
    {
        // 需要获得一个房间的中心坐标，来判断距离
        public RoomType RoomType;
        public Vector3 CenterWorldPosition;
    }


    public class MapManager : Singleton<MapManager>
    {
        private MapData _mapData;

        public List<MapTile> mapTileList;
        public Dictionary<RoomType, List<MapTile>> mapTileDic;
        public List<MapFurniture> mapFurnitureList;
        public Dictionary<RoomType, List<MapFurniture>> mapFurnitureDic;
        public List<MapWall> mapWallList;

        private readonly Dictionary<Vector2Int, RoomType> _grid2RoomType = new();

        public void Init(MapData mapData)
        {
            _mapData = mapData;
            mapTileList = new List<MapTile>();
            mapTileDic = new Dictionary<RoomType, List<MapTile>>();
            mapFurnitureList = new List<MapFurniture>();
            mapFurnitureDic = new Dictionary<RoomType, List<MapFurniture>>();
            mapWallList = new List<MapWall>();

            GenerateMap(mapData);
            BuildRoomLookup();
        }

        public void GenerateMap(MapData mapData)
        {
            if (mapData == null) return;
            if (GameManager.Instance.GameGeneratePoint == null) return;
            GameObject tmpRoot = GameObject.Find("Map");
            if (tmpRoot != null) DestroyImmediate(tmpRoot);
            GameObject root = new GameObject("Map");
            root.transform.position = GameManager.Instance.GameGeneratePoint.position;

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

                MapTile mapTile = tileGO.GetComponent<MapTile>();
                mapTile.SetData(tile);
                tileGO.transform.SetParent(tileRoot.transform);
                tileGO.transform.position += root.transform.position;
                mapTileList.Add(mapTile);
                if (mapTileDic.ContainsKey(tile.roomType))
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
                furnitureGO.transform.rotation = Quaternion.Euler(0, furniture.rotation, 0);
                MapFurniture mapFurniture = furnitureGO.GetComponent<MapFurniture>();
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
                //???????????λ
                for (int i = 0; i < wall.stackLayer; i++)
                {
                    GameObject wallGO = Instantiate(wall.wallData.wallPrefab,
                        new Vector3(averageX, 3 * i, -averageZ),
                        Quaternion.identity);
                    wallGO.transform.rotation = wall.wallData.wallPrefab.transform.rotation;
                    MapWall mapWall = wallGO.GetComponent<MapWall>();
                    mapWall.SetData(wall);
                    wallGO.transform.SetParent(wallRoot.transform);
                    wallGO.transform.position += root.transform.position;
                    mapWallList.Add(mapWall);
                }
            }
            tileRoot.isStatic = true;
            NavMeshSurface surface = tileRoot.AddComponent<NavMeshSurface>();
            surface.BuildNavMesh();
        }

        private void BuildRoomLookup()
        {
            _grid2RoomType.Clear();
            if (_mapData == null) return;

            foreach (var tile in _mapData.tileList)
            {
                _grid2RoomType[new Vector2Int(tile.mapPos.x, tile.mapPos.y)] = tile.roomType;
            }
        }


        /// <summary>
        /// 根据世界坐标判断是否在房间内。
        /// 若成功找到格子，则返回 true，并输出该格子的房间类型。
        /// </summary>
        public bool TryGetRoomTypeAtWorldPos(Vector3 worldPos, out RoomType roomType)
        {
            roomType = default;
            if (_grid2RoomType.Count == 0 || GameManager.Instance.GameGeneratePoint == null)
                return false;

            // 将世界坐标转换为格子坐标
            Vector3 local = worldPos - GameManager.Instance.GameGeneratePoint.position;
            int gridX = Mathf.RoundToInt(local.x);
            int gridY = Mathf.RoundToInt(-local.z);

            return _grid2RoomType.TryGetValue(new Vector2Int(gridX, gridY), out roomType);
        }
        
        /// <summary>
        /// 获取场景中所有房间的中心点与类型信息
        /// </summary>
        public List<RoomInfo> GetAllRooms()
        {
            List<RoomInfo> result = new List<RoomInfo>();

            foreach (var kvp in mapTileDic)
            {
                RoomType type = kvp.Key;
                List<MapTile> tiles = kvp.Value;
                if (tiles == null || tiles.Count == 0) continue;

                Vector3 sum = Vector3.zero;
                foreach (var tile in tiles)
                {
                    sum += tile.transform.position;
                }

                Vector3 avg = sum / tiles.Count;

                result.Add(new RoomInfo
                {
                    RoomType = type,
                    CenterWorldPosition = avg
                });
            }

            return result;
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