using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;

namespace KidGame.Core
{


    [CreateAssetMenu(fileName = "RecipeListData", menuName = "KidGameSO/Interactive/RecipeListData")]
    public class RecipeListData : ScriptableObject
    {
        public List<RecipeData> recipes = new List<RecipeData>();


    }
}