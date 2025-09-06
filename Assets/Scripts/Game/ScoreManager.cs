using KidGame.UI;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class ScoreManager : SingletonNoMono<ScoreManager>
    {
        //分数评级
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

        private int currentLoopScore;
        public void Init()
        {
            currentRating = ScoreRating.D;
            ratingProgress = 0f;
            currentCombo = 0;

            MsgCenter.RegisterMsgAct(MsgConst.ON_GAME_START, OnGameStart);
            MsgCenter.RegisterMsgAct(MsgConst.ON_GAME_FINISH, OnGameFinish);

        }

        public void Discard()
        {
            MsgCenter.UnregisterMsgAct(MsgConst.ON_GAME_START, OnGameStart);
            MsgCenter.UnregisterMsgAct(MsgConst.ON_GAME_FINISH, OnGameFinish);

        }


        #region 功能

        public void AddScore(int score)
        {
            lastScoreTime = Time.time;

            currentCombo++;
            comboWindowTimer = ComboWindowDuration;

            float progressIncrease = currentRating switch
            {
                ScoreRating.D => 30f,
                ScoreRating.C => 25f,
                ScoreRating.B => 20f,
                ScoreRating.A => 15f,
                ScoreRating.S => 10f,
                _ => 0f
            };
            // todo.

            ratingProgress = Mathf.Min(100f, ratingProgress + progressIncrease);

            if (ratingProgress >= 100f && currentRating < ScoreRating.S)
            {
                currentRating++;
                ratingProgress = 0f;
            }

            // todo.
            int comboScore = (int)(currentCombo * (1 + 0.1f * currentCombo) * 10);
            int totalScore = (int)(score * ratingMultipliers[currentRating]) + comboScore;

            currentLoopScore += totalScore;
            MsgCenter.SendMsg(MsgConst.ON_CUR_LOOP_SCORE_CHG, currentLoopScore);
            UIHelper.Instance.ShowOneFixedPosUIText(FixedUIPosType.Left, "+" + score, 0.75f);
        }

        private void UpdateRatingProgress()
        {
            if(GameManager.Instance.IsGamePaused)
                return;
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
        /// 
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

        private void OnGameStart()
        {
            this.OnUpdate(UpdateRatingProgress);
        }
        private void OnGameFinish()
        {
            this.RemoveUpdate(UpdateRatingProgress);
        }
    }

}