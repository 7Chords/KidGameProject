using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KidGame.Core
{
    public class LevelResManager : Singleton<LevelResManager>
    {

        private List<Furniture2MaterialMapping> _f2MMappingList;

        private List<Room2MaterialMapping> _r2MMappingList;

        private GameObject _materialRoot;
        public void Init()
        {
        }

        public void Discard()
        {
        }

        public void InitLevelRes(List<Furniture2MaterialMapping> f2MMappingList, List<Room2MaterialMapping> r2MMappingList)
        {
            _f2MMappingList = f2MMappingList;
            _r2MMappingList = r2MMappingList;

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
                var mapping = _f2MMappingList.Find(x =>
                    x.serialNumber.Equals(mapFurniture.mapFurnitureData.serialNumber));

                if (mapping != null)
                {
                    int totalGridCount = mapFurniture.mapFurnitureData.furnitureData.gridLayout.x *
                        mapFurniture.mapFurnitureData.furnitureData.gridLayout.y;

                    float spawnChance = Random.Range(mapping.gridSpawnMatChance_min, mapping.gridSpawnMatChance_max);

                    int spawnCount = Mathf.FloorToInt(totalGridCount * spawnChance);//向下取整

                    float totalMatSpawnChance = 0;

                    foreach (var cfg in mapping.materialDataList)
                    {
                        totalMatSpawnChance += cfg.spawnChance;
                    }

                    for(int i =0;i< spawnCount;i++)
                    {
                        float randomNum = Random.Range(0, totalMatSpawnChance);
                        float lower = 0f, upper = mapping.materialDataList[0].spawnChance;
                        for(int j = 0;j< mapping.materialDataList.Count;j++)
                        {
                            if (randomNum >= lower && randomNum < upper)
                            {
                                furnitureMaterials.Add(new MaterialItem
                                    (SoLoader.Instance.GetMaterialDataDataById(mapping.materialDataList[j].materialId),
                                    Random.Range(mapping.materialDataList[j].randomAmount_min, mapping.materialDataList[j].randomAmount_max + 1)));
                                break;
                            }
                            lower += mapping.materialDataList[j].spawnChance;
                            upper += mapping.materialDataList[j].spawnChance;
                        }
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
            foreach (var mapping in _r2MMappingList)
            {
                if (_materialRoot == null)
                {
                    _materialRoot = new GameObject("Material_Root");
                    _materialRoot.transform.position = GameManager.Instance.GameGeneratePoint.position;
                }

                GameObject materialGO = MaterialFactory.CreateEntity(SoLoader.Instance.GetMaterialDataDataById(mapping.materialId), 
                    new Vector3(mapping.spawnPos.x,mapping.spawnPos.y,-mapping.spawnPos.z),
                    Random.Range(mapping.randomAmount_min,mapping.randomAmount_max+1));
                materialGO.transform.SetParent(_materialRoot.transform);
                materialGO.transform.position += _materialRoot.transform.position;
            }
        }


    }
}