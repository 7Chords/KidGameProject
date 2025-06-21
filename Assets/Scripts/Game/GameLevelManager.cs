using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 一局游戏内的一个个关卡的管理器
    /// </summary>
    public class GameLevelManager : Singleton<GameLevelManager>
    {
        private bool _levelStarted;
        public bool levelStarted => _levelStarted;

        private bool _levelFinished;
        public bool levelFinished => _levelFinished;

        private List<GameLevelData> _levelDataList;

        public List<Transform> bornPointList;

        private int levelIndex;

        public void Init(List<GameLevelData> levelDataList)
        {
            _levelDataList = levelDataList;

            EnemyManager.Instance.Init();
        }

        public void InitFirstLevel()
        {
            EnemyManager.Instance.InitEnemy(_levelDataList[0].enemyDataList, bornPointList);
        }

        public void InitNextLevel()
        {
            levelIndex++;
            EnemyManager.Instance.InitEnemy(_levelDataList[levelIndex].enemyDataList, bornPointList);
        }

        public void DiscardLevel()
        {
            EnemyManager.Instance.DiscardEnemy();
        }

        public void StartLevel()
        {
            _levelStarted = true;
        }

        public void FinishLevel()
        {
            _levelFinished = true;
        }
    }
}