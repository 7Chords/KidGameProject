using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "KidGameSO/Game/GameData")]
public class GameData : ScriptableObject
{
    public List<GameLevelData> levelDataList;
}
