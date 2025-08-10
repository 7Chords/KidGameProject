using KidGame.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    [Serializable]
    public class MaterialResCfg
    {
        public string materialId;
        public int randomAmount_min;
        public int randomAmount_max;
        public float spawnChance;//���ɵĸ���

        public MaterialResCfg(string materialId, int randomAmount_min, int randomAmount_max, float spawnChance)
        {
            this.materialId = materialId;
            this.randomAmount_min = randomAmount_min;
            this.randomAmount_max = randomAmount_max;
            this.spawnChance = spawnChance;
        }
    }

    [Serializable]
    public class EnemySpawnCfg
    {
        public EnemyBaseData enemyData;
        public Vector3 enemySpawnPos;
    }

    [Serializable]
    //家具到材料的映射
    public class Furniture2MaterialMapping
    {
        [HideInInspector] public int serialNumber;//序列号 与地图数据中的家具一一对应
        public float gridSpawnMatChance_min;
        public float gridSpawnMatChance_max;
        public List<MaterialResCfg> materialDataList;
    }

    [Serializable]
    //房间到材料的映射
    public class Room2MaterialMapping
    {
        public Vector3 spawnPos;
        public string materialId;
        public int randomAmount_min;
        public int randomAmount_max;

        public Room2MaterialMapping(Vector3 spawnPos, string materialId, int randomAmount_min, int randomAmount_max)
        {
            this.spawnPos = spawnPos;
            this.materialId = materialId;
            this.randomAmount_min = randomAmount_min;
            this.randomAmount_max = randomAmount_max;
        }
    }

    [CreateAssetMenu(fileName = "GameLevelData", menuName = "KidGameSO/Game/GameLevelData")]
    public class GameLevelData : ScriptableObject
    {
        public List<EnemySpawnCfg> enemySpawnCfgList;

        public List<Furniture2MaterialMapping> f2MMappingList;

        public List<Room2MaterialMapping> r2MMappingList;

    }
}