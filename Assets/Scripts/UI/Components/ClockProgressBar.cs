using UnityEngine;
using UnityEngine.UI;
using KidGame.Core;

namespace KidGame.UI
{
    public class ClockProgressBar : ProgressBar
    {
        [Header("Clock Settings")]
        [SerializeField] private Image dayNightIcon;          // 白天/黑夜图标
        [SerializeField] private Sprite dayIcon;             // 白天图标
        [SerializeField] private Sprite nightIcon;           // 黑夜图标
        
        private GameLevelManager.LevelPhase _currentPhase;
        private bool _isFirstUpdate = true;

        protected override void Start()
        {
            base.Start();
            ResetForNewPhase(GameLevelManager.LevelPhase.Day);
        }

        public void UpdatePhaseTime(GameLevelManager.LevelPhase phase, float progressPercentage)
        {
            if (_currentPhase != phase || _isFirstUpdate)
            {
                ResetForNewPhase(phase);
                _isFirstUpdate = false;
            }
            
            base.SetProgress(progressPercentage);
        }

        private void ResetForNewPhase(GameLevelManager.LevelPhase phase)
        {
            _currentPhase = phase;
            
            Initialize();
            
            UpdateClockVisuals(phase);
            
            PlayPhaseChangeAnimation();
        }

        private void UpdateClockVisuals(GameLevelManager.LevelPhase phase)
        {
            if (dayNightIcon != null)
            {
                dayNightIcon.sprite = phase == GameLevelManager.LevelPhase.Day ? dayIcon : nightIcon;
            }
            
            UpdateProgressBarColor(phase);
        }

        private void UpdateProgressBarColor(GameLevelManager.LevelPhase phase)
        {
            if (fillImage != null)
            {
                fillImage.color = phase == GameLevelManager.LevelPhase.Day 
                    ? Color.yellow 
                    : new Color(0.2f, 0.2f, 0.8f);
            }
        }

        private void PlayPhaseChangeAnimation()
        {
            // todo.用dotween做一些效果
        }
    }
}