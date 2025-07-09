using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KidGame.Core
{
    public class LevelResManager : Singleton<LevelResManager>
    {
        private LevelResData _resData;

        private GameObject _materialRoot;

        public void Init()
        {
        }

        public void Discard()
        {
        }

        public void InitLevelRes(LevelResData resData)
        {
            _resData = resData;

            // 初始化有材料的家具
            InitializeFurnitureMaterials();

            // 生成房间里散落的材料
            SpawnRoomMaterials();
        }

        private void InitializeFurnitureMaterials()
        {
            var furnitureMaterials = new List<MaterialItem>(); // 每个家具有自己的列表
            foreach (var mapFurniture in MapManager.Instance.mapFurnitureList)
            {
                furnitureMaterials.Clear();
                var mapping = _resData.f2MMappingList.Find(x =>
                    x.furnitureData.Equals(mapFurniture.mapFurnitureData.furnitureData));

                if (mapping != null)
                {
                    foreach (var item in mapping.materialDataList)
                    {
                        furnitureMaterials.Add(new MaterialItem(
                            item.materialData,
                            Random.Range(item.randomAmount_min, item.randomAmount_max + 1)
                        ));
                    }
                    mapFurniture.Init(furnitureMaterials);
                }
                else
                {
                    mapFurniture.Init(null);
                }
            }
        }

        private void SpawnRoomMaterials()
        {
            foreach (var mapping in _resData.r2MMappingList)
            {
                // 获取所有地板格子
                var allTiles = MapManager.Instance.mapTileDic.Values.SelectMany(x => x).ToList();

                // 获取所有家具和墙
                var allFurniture = MapManager.Instance.mapFurnitureList;
                var allWalls = MapManager.Instance.mapWallList;

                // 获取可生成材料的有效位置
                List<MapTile> validTiles = GetValidSpawnTiles(allTiles, allFurniture, allWalls);

                // 计算要生成的材料总数
                int totalMaterials = CalculateTotalMaterials(mapping.materialDataList);

                // 确保不超过可用位置数量
                totalMaterials = Mathf.Min(totalMaterials, validTiles.Count);

                // 随机选择位置生成材料
                SpawnMaterialsAtRandomPositions(mapping.materialDataList, validTiles, totalMaterials);
            }
        }

        private List<MapTile> GetValidSpawnTiles(List<MapTile> roomTiles,
            List<MapFurniture> roomFurniture, List<MapWall> roomWalls)
        {
            List<MapTile> validTiles = new List<MapTile>();

            foreach (var tile in roomTiles)
            {
                bool isOccupied = false;

                // 检查是否有家具占用此位置
                foreach (var furniture in roomFurniture)
                {
                    if (furniture.mapFurnitureData.mapPosList.Contains(tile.mapTileData.mapPos))
                    {
                        isOccupied = true;
                        break;
                    }
                }

                // 检查是否有墙占用此位置
                if (!isOccupied)
                {
                    foreach (var wall in roomWalls)
                    {
                        if (wall.mapWallData.mapPosList.Contains(tile.mapTileData.mapPos))
                        {
                            isOccupied = true;
                            break;
                        }
                    }
                }

                if (!isOccupied)
                {
                    validTiles.Add(tile);
                }
            }

            return validTiles;
        }

        private int CalculateTotalMaterials(List<MaterialResCfg> materialResCfgList)
        {
            int total = 0;
            foreach (var resCfg in materialResCfgList)
            {
                total += Random.Range(resCfg.randomAmount_min, resCfg.randomAmount_max + 1);
            }

            return total;
        }

        private void SpawnMaterialsAtRandomPositions(List<MaterialResCfg> materialResCfgList,
            List<MapTile> validTiles, int totalMaterials)
        {
            // 先打乱有效位置列表
            ShuffleTiles(validTiles);

            // 按材料类型和数量生成
            int spawnedCount = 0;
            foreach (var materialData in materialResCfgList)
            {
                int amount = Random.Range(materialData.randomAmount_min, materialData.randomAmount_max + 1);

                for (int i = 0; i < amount && spawnedCount < totalMaterials; i++, spawnedCount++)
                {
                    if (spawnedCount >= validTiles.Count) break;

                    var tile = validTiles[spawnedCount];

                    SpawnMaterialAtPosition(materialData.materialData, tile.transform.position);
                }
            }
        }

        private void ShuffleTiles(List<MapTile> tiles)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                int randomIndex = Random.Range(i, tiles.Count);
                (tiles[i], tiles[randomIndex]) = (tiles[randomIndex], tiles[i]);
            }
        }

        private void SpawnMaterialAtPosition(MaterialData materialData, Vector3 position)
        {
            if(_materialRoot == null)
            {
                _materialRoot = new GameObject("Material_Root");
            }

            Vector3 placePosition = position + Vector3.up;
            GameObject materialGO = MaterialFactory.Create(materialData, placePosition);
            materialGO.transform.SetParent(_materialRoot.transform);
            MaterialBase materialBase = materialGO.GetComponent<MaterialBase>();
            materialBase.Init(materialData);
        }
    }
}