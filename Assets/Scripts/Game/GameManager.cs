using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;

namespace KidGame.Core
{
    public class GameManager : MonoBehaviour
    {
        public GameData gameData;

        //�ܵ���Ϸ��ʼ
        private bool _gameStarted;

        //�ܵ���Ϸ����
        private bool _gameFinished;

        private int _levelIndex;


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
            MapManager.Instance.Init();
            GameLevelManager.Instance.Init(gameData.levelDataList);
        }

        private void DiscardGame()
        {
            //��Ϸ����ģ������� ��Ҫ���¼��ķ�ע��
            PlayerManager.Instance.Discard();
            MapManager.Instance.Discard();
        }

        public void StartGame()
        {
            _gameStarted = true;
            GameLevelManager.Instance.InitLevel(0);
        }

        public void FinishGame()
        {
            _gameFinished = true;
        }
    }
}