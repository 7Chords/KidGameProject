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
        public float spawnChance;//生成的概率
    }

    [Serializable]
    public class EnemySpawnCfg
    {
        public EnemyBaseData enemyData;
        public Vector3 enemySpawnPos;
    }

    [Serializable]
    //家具中存在的道具的映射
    public class Furniture2MaterialMapping
    {
        [HideInInspector] public int serialNumber;//序列号 用于和家具列表的家具做对应
        public float gridSpawnMatChance_min;
        public float gridSpawnMatChance_max;
        public List<MaterialResCfg> materialDataList;
    }

    [Serializable]
    //房间中直接可以获取的道具的映射
    public class Room2MaterialMapping
    {
        public Vector3 spawnPos;
        public string materialId;
        public int randomAmount_min;
        public int randomAmount_max;
    }

    [CreateAssetMenu(fileName = "GameLevelData", menuName = "KidGameSO/Game/GameLevelData")]
    public class GameLevelData : ScriptableObject
    {
        public List<EnemySpawnCfg> enemySpawnCfgList;

        public List<Furniture2MaterialMapping> f2MMappingList;

        public List<Room2MaterialMapping> r2MMappingList;

    }
}