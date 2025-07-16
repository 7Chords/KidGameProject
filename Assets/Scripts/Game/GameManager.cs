using UnityEngine;
using KidGame.Core.Data;
using KidGame.UI;
using Utils;
using System;

namespace KidGame.Core
{
    public class GameManager : Singleton<GameManager>
    {
        public GameData GameData;

        //总的游戏开始
        private bool gameStarted;

        //总的游戏结束
        private bool gameFinished;


        private int levelIndex;

        //游戏暂停
        private bool isGamePuased;
        public bool IsGamePaused => isGamePuased;

        #region 分数变量

        // 用于本轮（白天+夜晚）的总得分
        private int currentLoopScore;

        public int CurrentLoopScore => currentLoopScore;

        public Action<int> OnCurrentLoopScoreChanged;

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
            GameLevelManager.Instance.Init(GameData.levelDataList);

            //todo
            GamePlayPanelController.Instance.Init();
            CameraController.Instance.Init();
        }

        private void DiscardGame()
        {
            //游戏各个模块的销毁 主要是事件的反注册
            PlayerManager.Instance.Discard();
            MapManager.Instance.Discard();
            GameLevelManager.Instance.Discard();


            //todo
            GamePlayPanelController.Instance.Discard();
            CameraController.Instance.Discard();

        }

        /// <summary>
        /// 开始游戏 保证在初始游戏之后调用
        /// </summary>
        public void StartGame()
        {
            gameStarted = true;
            GameLevelManager.Instance.InitFirstLevel();
            GameLevelManager.Instance.StartDayPhase();
        }

        public void FinishGame()
        {
            gameFinished = true;
        }
        public void GameOver()
        {
            gameFinished = true;
            Signals.Get<GameFailSignal>().Dispatch();
        }


        #endregion

        #region 分数统计

        // 外部调用，比如陷阱命中、击杀敌人时调用
        public void AddScore(int score)
        {
            currentLoopScore += score;
            OnCurrentLoopScoreChanged?.Invoke(currentLoopScore);
            UIHelper.Instance.ShowOneSildUIText("+" + score, 0.75f);
        }

        public int GetCurrentLoopScore()
        {
            return currentLoopScore;
        }

        // 清空分数方法，每个新白天开始时调用
        public void ResetLoopScore()
        {
            currentLoopScore = 0;
            OnCurrentLoopScoreChanged?.Invoke(currentLoopScore);
        }

        #endregion

        #region 游戏暂停
        public void GamePause()
        {
            isGamePuased = true;
            Time.timeScale = 0;
        }
        public void GameResume()
        {
            isGamePuased = false;
            Time.timeScale = 1;
        }
        #endregion
    }
}