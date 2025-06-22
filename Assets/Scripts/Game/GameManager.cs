using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;
using KidGame.Core.Data;

namespace KidGame.Core
{
    public class GameManager : MonoBehaviour
    {
        public GameData gameData;

        //�ܵ���Ϸ��ʼ
        private bool _gameStarted;

        //�ܵ���Ϸ����
        private bool _gameFinished;

        //test public
        public MapData mapData;

        //Start���ñ�֤����������ɳ�ʼ��
        private void Start()
        {
            InitGame();
            //��ʱ���� 
            StartGame();
        }


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
        }

        public void FinishGame()
        {
            _gameFinished = true;
        }
    }
}