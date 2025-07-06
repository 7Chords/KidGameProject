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
        public TrapData trapData;
        //֮���������������
        public RecipeType recipeType;
        
        public List<MaterialSlotInfo> materialDatas;
        
        
    }
}
