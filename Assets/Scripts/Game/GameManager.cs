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

        //�ܵ���Ϸ��ʼ
        private bool gameStarted;

        //�ܵ���Ϸ����
        private bool gameFinished;


        private int levelIndex;

        //��Ϸ��ͣ
        private bool isGamePuased;
        public bool IsGamePaused => isGamePuased;

        #region ��������

        // ���ڱ��֣�����+ҹ�����ܵ÷�
        private int currentLoopScore;

        public int CurrentLoopScore => currentLoopScore;

        public Action<int> OnCurrentLoopScoreChanged;

        #endregion

        //test public
        public MapData mapData;

        public bool TestMode;//�������ڸó����Ϳ���ֱ��������Ϸ���У���ʱ����һЩ��Ҫ��ʼ���������Ķ�����

        //Start���ñ�֤����������ɳ�ʼ��
        private void Start()
        {
            InitGame();
            //��ʱ���� 
            StartGame();
        }

        #region ��Ϸѭ��

        private void InitGame()
        {
            //��Ϸ����ģ��ĳ�ʼ��
            //������������Ϸ����Ҫ��ȡ�浵����
            PlayerManager.Instance.Init();
            MapManager.Instance.Init(mapData);
            GameLevelManager.Instance.Init(GameData.levelDataList);

            //todo
            GamePlayPanelController.Instance.Init();
            CameraController.Instance.Init();
        }

        private void DiscardGame()
        {
            //��Ϸ����ģ������� ��Ҫ���¼��ķ�ע��
            PlayerManager.Instance.Discard();
            MapManager.Instance.Discard();
            GameLevelManager.Instance.Discard();


            //todo
            GamePlayPanelController.Instance.Discard();
            CameraController.Instance.Discard();

        }

        /// <summary>
        /// ��ʼ��Ϸ ��֤�ڳ�ʼ��Ϸ֮�����
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

        #region ����ͳ��

        // �ⲿ���ã������������С���ɱ����ʱ����
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

        // ��շ���������ÿ���°��쿪ʼʱ����
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