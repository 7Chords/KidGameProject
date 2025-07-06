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
    [Serializable]
    public class MaterialRequirement
    {
        public MaterialData material;
        public int count;
    }
    [CreateAssetMenu(fileName = "RecipeData", menuName = "KidGameSO/Interactive/RecipeData")]
    public class RecipeData : ScriptableObject
    {
        public TrapData trapData;
        //֮���������������
        public RecipeType recipeType;
        
        public List<MaterialRequirement> materialDatas;
        
        
    }
}
