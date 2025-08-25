using UnityEngine;
using UnityEngine.UI;
using KidGame.Core;

namespace KidGame.UI
{
    public class ClockProgressBar : ProgressBar
    {
        [Header("Clock Settings")]
        [SerializeField] private Image dayNightIcon;          // ����/��ҹͼ��
        [SerializeField] private Sprite dayIcon;             // ����ͼ��
        [SerializeField] private Sprite nightIcon;           // ��ҹͼ��
        
        private LevelPhase _currentPhase;
        private bool _isFirstUpdate = true;

        protected override void Start()
        {
            base.Start();
            ResetForNewPhase(LevelPhase.Day);
        }

        public void UpdatePhaseTime(LevelPhase phase, float progressPercentage)
        {
            if (_currentPhase != phase || _isFirstUpdate)
            {
                ResetForNewPhase(phase);
                _isFirstUpdate = false;
            }
            
            base.SetProgress(progressPercentage);
        }

        private void ResetForNewPhase(LevelPhase phase)
        {
            _currentPhase = phase;
            
            Initialize();
            
            UpdateClockVisuals(phase);
            
            PlayPhaseChangeAnimation();
        }

        private void UpdateClockVisuals(LevelPhase phase)
        {
            if (dayNightIcon != null)
            {
                dayNightIcon.sprite = phase == LevelPhase.Day ? dayIcon : nightIcon;
            }
            
            UpdateProgressBarColor(phase);
        }

        private void UpdateProgressBarColor(LevelPhase phase)
        {
            if (fillImage != null)
            {
                fillImage.color = phase == LevelPhase.Day 
                    ? Color.yellow 
                    : new Color(0.2f, 0.2f, 0.8f);
            }
        }

        private void PlayPhaseChangeAnimation()
        {
            // todo.��dotween��һЩЧ��
        }
    }
}