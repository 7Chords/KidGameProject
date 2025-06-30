using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KidGame.Core
{
    public class LevelResManager : Singleton<LevelResManager>
    {
        private LevelResData _resData;

        public void Init()
        {
        }

        public void Discard()
        {
        }

        public void InitLevelRes(LevelResData resData)
        {
            _resData = resData;
            List<MaterialItem> tmpMaterialItemList = new List<MaterialItem>();

            // 初始化有材料的家具
            InitializeFurnitureMaterials(tmpMaterialItemList);

            // 生成房间里散落的材料
            SpawnRoomMaterials();
        }

        private void InitializeFurnitureMaterials(List<MaterialItem> tmpMaterialItemList)
        {
            foreach (var mapFurniture in MapManager.Instance.mapFurnitureList)
            {
                tmpMaterialItemList.Clear();
                var mapping = _resData.f2MMappingList.Find(x =>
                    x.furnitureData.Equals(mapFurniture.mapFurnitureData.furnitureData));

                if (mapping != null)
                {
                    foreach (var item in mapping.materialDataList)
                    {
                        tmpMaterialItemList.Add(new MaterialItem(
                            item.materialData,
                            Random.Range(item.randomAmount_min, item.randomAmount_max + 1)
                        ));
                    }

                    mapFurniture.Init(true, tmpMaterialItemList);
                }
            }
        }

        private void SpawnRoomMaterials()
        {
            foreach (var mapping in _resData.r2MMappingList)
            {
                var roomTiles = MapManager.Instance.mapTileDic[mapping.roomType];
                var roomFurniture = MapManager.Instance.mapFurnitureDic[mapping.roomType];
                var roomWalls = MapManager.Instance.mapWallList;

                // 获取可生成材料的有效位置
                List<MapTile> validTiles = GetValidSpawnTiles(roomTiles, roomFurniture, roomWalls);

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
            Vector3 placePosition = position + Vector3.up;
            GameObject materialGO = MaterialFactory.Create(materialData, placePosition);
            MaterialBase materialBase = materialGO.GetComponent<MaterialBase>();
            materialBase.Init(materialData);
            Debug.Log(materialGO + "生成在" + position);
        }
    }
}