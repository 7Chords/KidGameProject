using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public enum RecipeType
    {
        None,
        Trap,
        Equip,
        Mix,
    }

    [CreateAssetMenu(fileName = "RecipeData", menuName = "KidGameSO/Interactive/RecipeData")]
    public class RecipeData : ScriptableObject
    {
        public bool isUnlocked = false;
        
        public TrapData trapData;

        //之后放其他类型数据
        public RecipeType recipeType;

        public List<MaterialSlotInfo> materialDatas;
    }
}