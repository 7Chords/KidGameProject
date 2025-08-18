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
        
        private bool gameStarted; // �ܵ���Ϸ��ʼ
        private bool gameFinished; // �ܵ���Ϸ����

        private int levelIndex;

        // ��Ϸ��ͣ
        private bool isGamePuased;
        public bool IsGamePaused => isGamePuased;

        public Transform GameGeneratePoint;

        #region ��������

        private int currentLoopScore;

        public Action<int> OnCurrentLoopScoreChanged;

        // �÷�����
        public enum ScoreRating
        {
            D,
            C,
            B,
            A,
            S
        }

        private ScoreRating currentRating = ScoreRating.D;
        private float ratingProgress = 0f; // ��������
        private float lastScoreTime = 0f;

        private readonly Dictionary<ScoreRating, float> ratingMultipliers = new Dictionary<ScoreRating, float>
        {
            { ScoreRating.D, 30f },
            { ScoreRating.C, 25f },
            { ScoreRating.B, 20f },
            { ScoreRating.A, 15f },
            { ScoreRating.S, 10f }
        }; // ��ͬ�����ķ�������

        private readonly Dictionary<ScoreRating, float> ratingDecayRates = new Dictionary<ScoreRating, float>
        {
            { ScoreRating.D, 1f },
            { ScoreRating.C, 2f },
            { ScoreRating.B, 4f },
            { ScoreRating.A, 5f },
            { ScoreRating.S, 7f }
        };// ��ͬ�����ķ����˼��ٶ�

        // Combo
        private int currentCombo = 0;
        private float comboWindowTimer = 0f;
        private const float ComboWindowDuration = 3f; // 3������

        #endregion

        public Action OnGameStarted;
        public Action OnGameFinished;
        public Action OnGameOver;

        public MapData mapData;
        public bool TestMode; // ����ģʽ

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

        #region ��Ϸѭ��

        private void InitGame()
        {
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
        /// ��ʼ��Ϸ
        /// </summary>
        public void StartGame()
        {
            if (gameStarted) return;

            gameStarted = true;
            GameLevelManager.Instance.InitFirstLevel();
            
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
            Signals.Get<GameFailSignal>().Dispatch(); // ������ʾUI
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
            OnCurrentLoopScoreChanged?.Invoke(currentLoopScore);
            UIHelper.Instance.ShowOneSildUIText("+" + score, 0.75f);
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
            OnCurrentLoopScoreChanged?.Invoke(currentLoopScore);
            
            ResetCombo();
        }
        
        private void ResetCombo()
        {
            currentCombo = 0;
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