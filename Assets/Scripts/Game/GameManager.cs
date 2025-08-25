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

        #region 分数

        private int currentLoopScore;

        public enum ScoreRating
        {
            D,
            C,
            B,
            A,
            S
        }

        private ScoreRating currentRating = ScoreRating.D;
        private float ratingProgress = 0f;
        private float lastScoreTime = 0f;

        private readonly Dictionary<ScoreRating, float> ratingMultipliers = new Dictionary<ScoreRating, float>
        {
            { ScoreRating.D, 30f },
            { ScoreRating.C, 25f },
            { ScoreRating.B, 20f },
            { ScoreRating.A, 15f },
            { ScoreRating.S, 10f }
        };

        private readonly Dictionary<ScoreRating, float> ratingDecayRates = new Dictionary<ScoreRating, float>
        {
            { ScoreRating.D, 1f },
            { ScoreRating.C, 2f },
            { ScoreRating.B, 4f },
            { ScoreRating.A, 5f },
            { ScoreRating.S, 7f }
        };

        // Combo
        private int currentCombo = 0;
        private float comboWindowTimer = 0f;
        private const float ComboWindowDuration = 3f;

        #endregion


        public MapData mapData;
        [Header("测试模式 需要打开才能直接启动")]
        public bool TestMode;

        private void Start()
        {
            InitGame();
            if (TestMode) GameStart();
        }

        private void Update()
        {
            if (isGamePuased) return;

            UpdateRatingProgress();
        }

        #region ��Ϸѭ��

        private void InitGame()
        {
            SoLoader.Instance.InitialSoResource();


            MapManager.Instance.Init(mapData);
            GameLevelManager.Instance.Init(GameData.levelDataList);
            PlayerManager.Instance.Init(GameData.levelDataList[0].playerSpawnPos);

            // UI ��ʼ��
            GamePlayPanelController.Instance.Init();
            CameraController.Instance.Init();

            currentRating = ScoreRating.D;
            ratingProgress = 0f;
            currentCombo = 0;

            GameObject.Find("Map").transform.rotation = GameGeneratePoint.transform.rotation;
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

        #region ����ͳ��

        public void AddScore(int score)
        {
            lastScoreTime = Time.time;

            currentCombo++;
            comboWindowTimer = ComboWindowDuration;
            
            // ������������
            float progressIncrease = currentRating switch
            {
                ScoreRating.D => 30f,
                ScoreRating.C => 25f,
                ScoreRating.B => 20f,
                ScoreRating.A => 15f,
                ScoreRating.S => 10f,
                _ => 0f
            };
            // todo.������ֳֵ���
            
            ratingProgress = Mathf.Min(100f, ratingProgress + progressIncrease);
            
            if (ratingProgress >= 100f && currentRating < ScoreRating.S)
            {
                currentRating++;
                ratingProgress = 0f;
            }
            
            // ������/�ֳֵ��ߵ÷�*�÷��������� + combo���� *��1+0.1*combo������* 10 + �ر���ϵ÷֣�*��1+ �ֻű��ʣ�
            // todo.���ӿֻű��ʵķ���Ӱ��
            int comboScore = (int)(currentCombo * (1 + 0.1f * currentCombo) * 10);
            int totalScore = (int)(score * ratingMultipliers[currentRating]) + comboScore;
            
            currentLoopScore += totalScore;
            MsgCenter.SendMsg(MsgConst.ON_CUR_LOOP_SCORE_CHG, currentLoopScore);
            UIHelper.Instance.ShowOneFixedPosUIText(FixedUIPosType.Left,"+" + score, 0.75f);
        }
        
        private void UpdateRatingProgress()
        {
            // ��һ��ʱ����û�д��������ʹ���ֳֵ��ߣ����ڽ��Ⱦͻ��½�
            if (Time.time - lastScoreTime > 1f)
            {
                ratingProgress = Mathf.Max(0, ratingProgress - ratingDecayRates[currentRating] * Time.deltaTime);
                
                if (ratingProgress <= 0 && currentRating > ScoreRating.D)
                {
                    currentRating--;
                    ratingProgress = 99f;
                }
            }
        }
        
        /// <summary>
        /// ������ץסʱ�۷�
        /// </summary>
        public void DeductScore()
        {
            int deduction = (int)(currentLoopScore * 0.05f);
            currentLoopScore = Mathf.Max(0, currentLoopScore - deduction);
            MsgCenter.SendMsg(MsgConst.ON_CUR_LOOP_SCORE_CHG, currentLoopScore);

            ResetCombo();
        }
        
        private void ResetCombo()
        {
            currentCombo = 0;
        }

        #endregion

        #region 暂停游戏相关

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