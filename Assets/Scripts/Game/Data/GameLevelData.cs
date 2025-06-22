using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    [CreateAssetMenu(fileName = "GameLevelData", menuName = "KidGameSO/Game/GameLevelData")]
    public class GameLevelData : ScriptableObject
    {
        public List<EnemyBaseData> enemyDataList;

        public LevelResData levelResData;
    }
}