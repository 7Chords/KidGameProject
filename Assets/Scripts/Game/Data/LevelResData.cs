using KidGame.Core.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KidGame.Core
{
    [Serializable]
    public class MaterialResCfg
    {
        public MaterialData materialData;
        public int randomAmount_min;
        public int randomAmount_max;
    }

    [Serializable]
    //�Ҿ��д��ڵĵ��ߵ�ӳ��
    public class Furniture2MaterialMapping
    {
        public FurnitureData furnitureData;
        public List<MaterialResCfg> materialDataList;
    }

    [Serializable]
    //������ֱ�ӿ��Ի�ȡ�ĵ��ߵ�ӳ��
    public class Room2MaterialMapping
    {
        public RoomType roomType;
        public List<MaterialResCfg> materialDataList;
    }


    [CreateAssetMenu(fileName = "LevelResData", menuName = "KidGameSO/Game/LevelResData")]
    public class LevelResData : ScriptableObject
    {
        public List<Furniture2MaterialMapping> f2MMappingList;
        public List<Room2MaterialMapping> r2MMappingList;
    }
}