using UnityEngine;
using KidGame.Core.Data;
using KidGame.UI;
using Utils;

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
        }

        private void DiscardGame()
        {
            //��Ϸ����ģ������� ��Ҫ���¼��ķ�ע��
            PlayerManager.Instance.Discard();
            MapManager.Instance.Discard();
            GameLevelManager.Instance.Discard();
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

        #endregion

        #region ����ͳ��

        // �ⲿ���ã������������С���ɱ����ʱ����
        public void AddScore(int score)
        {
            currentLoopScore += score;
        }

        public int GetCurrentLoopScore()
        {
            return _currentLoopScore;
        }

        // ��շ���������ÿ���°��쿪ʼʱ����
        public void ResetLoopScore()
        {
            _currentLoopScore = 0;
        }

        #endregion

        #region ������
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