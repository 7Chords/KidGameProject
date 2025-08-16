using UnityEngine;
using KidGame.Core.Data;
using KidGame.UI;
using Utils;
using System;
using System.Collections.Generic;

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

        public Transform GameGeneratePoint;

        #region 分数变量

        private int currentLoopScore;

        public Action<int> OnCurrentLoopScoreChanged;

        // 得分评级
        public enum ScoreRating
        {
            D,
            C,
            B,
            A,
            S
        }

        private ScoreRating currentRating = ScoreRating.D;
        private float ratingProgress = 0f; // 评级进度
        private float lastScoreTime = 0f;

        private readonly Dictionary<ScoreRating, float> ratingMultipliers = new Dictionary<ScoreRating, float>
        {
            { ScoreRating.D, 30f },
            { ScoreRating.C, 25f },
            { ScoreRating.B, 20f },
            { ScoreRating.A, 15f },
            { ScoreRating.S, 10f }
        }; // 不同评级的分数倍率

        private readonly Dictionary<ScoreRating, float> ratingDecayRates = new Dictionary<ScoreRating, float>
        {
            { ScoreRating.D, 1f },
            { ScoreRating.C, 2f },
            { ScoreRating.B, 4f },
            { ScoreRating.A, 5f },
            { ScoreRating.S, 7f }
        };// 不同评级的分数退减速度

        // Combo
        private int currentCombo = 0;
        private float comboWindowTimer = 0f;
        private const float ComboWindowDuration = 3f; // 3秒重置

        #endregion

        public Action OnGameStarted;
        public Action OnGameFinished;
        public Action OnGameOver;

        public MapData mapData;
        public bool TestMode; // 测试模式

        private void Start()
        {
            InitGame();
            if (TestMode) StartGame();
        }

        private void Update()
        {
            if (isGamePuased) return;

            UpdateRatingProgress();
        }

        #region 游戏循环

        private void InitGame()
        {
            MapManager.Instance.Init(mapData);
            GameLevelManager.Instance.Init(GameData.levelDataList);
            PlayerManager.Instance.Init(GameData.levelDataList[0].playerSpawnPos);

            // UI 初始化
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
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            if (gameStarted) return;

            gameStarted = true;
            GameLevelManager.Instance.InitFirstLevel();
            
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
            Signals.Get<GameFailSignal>().Dispatch(); // 用于显示UI
        }

        #endregion

        #region 分数统计

        public void AddScore(int score)
        {
            lastScoreTime = Time.time;

            currentCombo++;
            comboWindowTimer = ComboWindowDuration;
            
            // 评级进度增加
            float progressIncrease = currentRating switch
            {
                ScoreRating.D => 30f,
                ScoreRating.C => 25f,
                ScoreRating.B => 20f,
                ScoreRating.A => 15f,
                ScoreRating.S => 10f,
                _ => 0f
            };
            // todo.如果是手持道具
            
            ratingProgress = Mathf.Min(100f, ratingProgress + progressIncrease);
            
            if (ratingProgress >= 100f && currentRating < ScoreRating.S)
            {
                currentRating++;
                ratingProgress = 0f;
            }
            
            // （陷阱/手持道具得分*得分评级倍率 + combo数量 *（1+0.1*combo数量）* 10 + 特别组合得分）*（1+ 恐慌倍率）
            // todo.添加恐慌倍率的分数影响
            int comboScore = (int)(currentCombo * (1 + 0.1f * currentCombo) * 10);
            int totalScore = (int)(score * ratingMultipliers[currentRating]) + comboScore;
            
            currentLoopScore += totalScore;
            OnCurrentLoopScoreChanged?.Invoke(currentLoopScore);
            UIHelper.Instance.ShowOneSildUIText("+" + score, 0.75f);
        }
        
        private void UpdateRatingProgress()
        {
            // 若一段时间内没有触发陷阱或使用手持道具，窗口进度就会下降
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
        /// 被敌人抓住时扣分
        /// </summary>
        public void DeductScore()
        {
            int deduction = (int)(currentLoopScore * 0.05f);
            currentLoopScore = Mathf.Max(0, currentLoopScore - deduction);
            OnCurrentLoopScoreChanged?.Invoke(currentLoopScore);
            
            ResetCombo();
        }
        
        private void ResetCombo()
        {
            currentCombo = 0;
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