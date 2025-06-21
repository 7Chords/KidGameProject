using KidGame.Core.Data;
using UnityEngine;

namespace KidGame.Core
{
    public class MapManager : Singleton<MapManager>
    {
        private MapData _mapData;


        public Transform MapGeneratePoint;
        public void Init(MapData mapData)
        {
            _mapData = mapData;
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
                }
            }
        }

        public void Discard()
        {

        }
    }
}
