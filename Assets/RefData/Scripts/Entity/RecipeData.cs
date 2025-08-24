using KidGame.Core;
using System;
using System.Collections.Generic;

[Serializable]
public class RecipeCell
{
    public string materialId;
    public int materialAmount;

    public RecipeCell(string materialId, int materialAmount)
    {
        this.materialId = materialId;
        this.materialAmount = materialAmount;
    }

}

[Serializable]
public class RecipeData_NoUse
{
    public string id;
    public string targetId;
    public RecipeType recipeType;
    public List<RecipeCell> materialList;
}

