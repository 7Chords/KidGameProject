using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    [CreateAssetMenu(fileName = "GameData", menuName = "KidGameSO/Game/GameData")]
    public class GameData : ScriptableObject
    {
        public List<GameLevelData> levelDataList;
    }
}
