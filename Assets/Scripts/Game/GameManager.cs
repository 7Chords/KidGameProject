using UnityEngine;
using KidGame.Core.Data;
using KidGame.UI;
using Utils;

namespace KidGame.Core
{
    public class GameManager : Singleton<GameManager>
    {
        public GameData gameData;

        //总的游戏开始
        private bool _gameStarted;

        //总的游戏结束
        private bool _gameFinished;


        private int _levelIndex;

        #region 分数变量

        // 用于本轮（白天+夜晚）的总得分
        private int _currentLoopScore;

        public int CurrentLoopScore => _currentLoopScore;

        #endregion

        //test public
        public MapData mapData;

        public bool TestMode;//启动后在该场景就可以直接启动游戏运行（临时生成一些需要开始场景带来的东西）

        //Start调用保证各个单例完成初始化
        private void Start()
        {
            InitGame();
            //临时测试 
            StartGame();
        }

        #region 游戏循环

        private void InitGame()
        {
            //游戏各个模块的初始化
            //如果点击继续游戏，需要读取存档数据
            PlayerManager.Instance.Init();
            MapManager.Instance.Init(mapData);
            GameLevelManager.Instance.Init(gameData.levelDataList);
        }

        private void DiscardGame()
        {
            //游戏各个模块的销毁 主要是事件的反注册
            PlayerManager.Instance.Discard();
            MapManager.Instance.Discard();
            GameLevelManager.Instance.Discard();
        }

        /// <summary>
        /// 开始游戏 保证在初始游戏之后调用
        /// </summary>
        public void StartGame()
        {
            _gameStarted = true;
            GameLevelManager.Instance.InitFirstLevel();
            GameLevelManager.Instance.StartDayPhase();
        }

        public void FinishGame()
        {
            _gameFinished = true;
        }

        #endregion

        #region 分数统计

        // 外部调用，比如陷阱命中、击杀敌人时调用
        public void AddScore(int score)
        {
            _currentLoopScore += score;
        }

        public int GetCurrentLoopScore()
        {
            return _currentLoopScore;
        }

        // 清空分数方法，每个新白天开始时调用
        public void ResetLoopScore()
        {
            _currentLoopScore = 0;
        }

        #endregion
    }
}