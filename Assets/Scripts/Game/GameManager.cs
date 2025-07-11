using UnityEngine;
using KidGame.Core.Data;
using KidGame.UI;
using Utils;

namespace KidGame.Core
{
    public class GameManager : Singleton<GameManager>
    {
        public GameData gameData;

        //�ܵ���Ϸ��ʼ
        private bool _gameStarted;

        //�ܵ���Ϸ����
        private bool _gameFinished;


        private int _levelIndex;

        #region ��������

        // ���ڱ��֣�����+ҹ�����ܵ÷�
        private int _currentLoopScore;

        public int CurrentLoopScore => _currentLoopScore;

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
            GameLevelManager.Instance.Init(gameData.levelDataList);
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
            _gameStarted = true;
            GameLevelManager.Instance.InitFirstLevel();
            GameLevelManager.Instance.StartDayPhase();
        }

        public void FinishGame()
        {
            _gameFinished = true;
        }

        #endregion

        #region ����ͳ��

        // �ⲿ���ã������������С���ɱ����ʱ����
        public void AddScore(int score)
        {
            _currentLoopScore += score;
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
    }
}