using KidGame.core;
using KidGame.Core.Data;
using KidGame.UI;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace KidGame.Core
{
    public class GameManager : Singleton<GameManager>
    {
        public GameData GameData;
        
        private bool gameStarted;
        private bool gameFinished;

        private bool isGamePuased;
        public bool IsGamePaused => isGamePuased;

        public Transform GameGeneratePoint;


        [Header("测试模式 需要打开才能直接启动")]
        public bool TestMode;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            InitGame();
            if (TestMode) GameStart();
        }


        #region 生命周期

        private void InitGame()
        {
            SoLoader.Instance.Init();
            GameModel.Instance.Init(GameData.playerData);

            MapManager.Instance.Init(GameData.mapData);
            GameLevelManager.Instance.Init(GameData.levelDataList);
            PlayerManager.Instance.Init(GameData.levelDataList[0].playerSpawnPos);
            ScoreManager.Instance.Init();


            GamePlayPanelController.Instance.Init();
            CameraController.Instance.Init();

            GameObject.Find("Map").transform.rotation = GameGeneratePoint.transform.rotation;
        }

        private void DiscardGame()
        {
            PlayerManager.Instance.Discard();
            MapManager.Instance.Discard();
            GameLevelManager.Instance.Discard();
            ScoreManager.Instance.Discard();

            GamePlayPanelController.Instance.Discard();
            CameraController.Instance.Discard();
        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        public void GameStart()
        {
            if (gameStarted) return;
            gameStarted = true;
            GameLevelManager.Instance.InitFirstLevel();

            MsgCenter.SendMsgAct(MsgConst.ON_GAME_START);


        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <param name="isWin">是否获胜</param>
        public void GameFinish(bool isWin)
        {
            if (gameFinished) return;
            gameFinished = true;
            MsgCenter.SendMsg(MsgConst.ON_GAME_FINISH, isWin);
            if (!isWin)
                Signals.Get<GameFailSignal>().Dispatch();
        }

        #endregion

        #region 暂停游戏相关

        public void GamePause()
        {
            isGamePuased = true;
            Time.timeScale = 0;
            MsgCenter.SendMsg(MsgConst.ON_CONTROL_MAP_CHG, ControlMap.UIMap);
        }

        public void GameResume()
        {
            isGamePuased = false;
            Time.timeScale = 1;
            MsgCenter.SendMsg(MsgConst.ON_CONTROL_MAP_CHG, ControlMap.GameMap);
        }

        #endregion
    }
}