using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "KidGameSO/Game/GameData")]
    public class GameData : ScriptableObject
    {
        public List<GameLevelData> levelDataList;

        public MapData mapData;

        public PlayerBaseData playerData;
    }
}
