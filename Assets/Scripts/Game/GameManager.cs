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
        
        private bool gameStarted; // 总的游戏开始
        private bool gameFinished; // 总的游戏结束
        
        private int levelIndex;

        // 游戏暂停
        private bool isGamePuased;
        public bool IsGamePaused => isGamePuased;

        #region 分数变量
        private int currentLoopScore;
        public int CurrentLoopScore => currentLoopScore;
        public Action<int> OnCurrentLoopScoreChanged;
        #endregion
        
        public Action OnGameStarted;
        public Action OnGameFinished;
        public Action OnGameOver;

        public MapData mapData;
        public bool TestMode; // 测试模式

        private void Start()
        {
            InitGame();
            if (TestMode) StartGame(); // 测试模式下直接开始游戏
        }

        #region 游戏循环
        private void InitGame()
        {
            PlayerManager.Instance.Init();
            MapManager.Instance.Init(mapData);
            GameLevelManager.Instance.Init(GameData.levelDataList);

            // UI 初始化
            GamePlayPanelController.Instance.Init();
            CameraController.Instance.Init();
        }

        private void DiscardGame()
        {
            PlayerManager.Instance.Discard();
            MapManager.Instance.Discard();
            GameLevelManager.Instance.Discard();
            
            GamePlayPanelController.Instance.Discard();
            CameraController.Instance.Discard();
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            if (gameStarted) return; // 避免重复调用

            gameStarted = true;
            GameLevelManager.Instance.InitFirstLevel();
            GameLevelManager.Instance.StartDayPhase();

            // 触发游戏开始事件
            OnGameStarted?.Invoke();
        }

        /// <summary>
        /// 结束游戏
        /// </summary>
        public void FinishGame()
        {
            if (gameFinished) return;

            gameFinished = true;
            OnGameFinished?.Invoke();
        }

        /// <summary>
        /// 游戏失败
        /// </summary>
        public void GameOver()
        {
            if (gameFinished) return;

            gameFinished = true;
            OnGameOver?.Invoke();
            Signals.Get<GameFailSignal>().Dispatch();
        }
        #endregion

        #region 分数统计
        public void AddScore(int score)
        {
            currentLoopScore += score;
            OnCurrentLoopScoreChanged?.Invoke(currentLoopScore);
            UIHelper.Instance.ShowOneSildUIText("+" + score, 0.75f);
        }

        public int GetCurrentLoopScore() => currentLoopScore;

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