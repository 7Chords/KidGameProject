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
        
        private bool gameStarted; // �ܵ���Ϸ��ʼ
        private bool gameFinished; // �ܵ���Ϸ����
        
        private int levelIndex;

        // ��Ϸ��ͣ
        private bool isGamePuased;
        public bool IsGamePaused => isGamePuased;

        #region ��������
        private int currentLoopScore;
        public int CurrentLoopScore => currentLoopScore;
        public Action<int> OnCurrentLoopScoreChanged;
        #endregion
        
        public Action OnGameStarted;
        public Action OnGameFinished;
        public Action OnGameOver;

        public MapData mapData;
        public bool TestMode; // ����ģʽ

        private void Start()
        {
            InitGame();
            if (TestMode) StartGame(); // ����ģʽ��ֱ�ӿ�ʼ��Ϸ
        }

        #region ��Ϸѭ��
        private void InitGame()
        {
            PlayerManager.Instance.Init();
            MapManager.Instance.Init(mapData);
            GameLevelManager.Instance.Init(GameData.levelDataList);

            // UI ��ʼ��
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
        /// ��ʼ��Ϸ
        /// </summary>
        public void StartGame()
        {
            if (gameStarted) return; // �����ظ�����

            gameStarted = true;
            GameLevelManager.Instance.InitFirstLevel();
            GameLevelManager.Instance.StartDayPhase();

            // ������Ϸ��ʼ�¼�
            OnGameStarted?.Invoke();
        }

        /// <summary>
        /// ������Ϸ
        /// </summary>
        public void FinishGame()
        {
            if (gameFinished) return;

            gameFinished = true;
            OnGameFinished?.Invoke();
        }

        /// <summary>
        /// ��Ϸʧ��
        /// </summary>
        public void GameOver()
        {
            if (gameFinished) return;

            gameFinished = true;
            OnGameOver?.Invoke();
            Signals.Get<GameFailSignal>().Dispatch();
        }
        #endregion

        #region ����ͳ��
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

        #region ��Ϸ��ͣ
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