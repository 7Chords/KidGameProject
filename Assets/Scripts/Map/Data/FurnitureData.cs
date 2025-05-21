using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GridPos
{
    public int x, y;

    public GridPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

[CreateAssetMenu(fileName = "FurnitureData", menuName = "KidGameSO/Map/FurnitureData")]
public class FurnitureData : ScriptableObject
{
    public string FurnitureName;

    public List<GridPos> posList;
}
