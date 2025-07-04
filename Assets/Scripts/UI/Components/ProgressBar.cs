using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected Image fillImage;
        [SerializeField] protected Image delayedFillImage;
        [SerializeField] protected Gradient progressGradient;
        
        [Header("Settings")]
        [SerializeField] private float delayDuration = 0.5f;
        
        private float _currentFillAmount = 1f;
        private float _targetFillAmount = 1f;
        private float _delayTimer = 0f;


        protected virtual void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (_delayTimer > 0)
            {
                _delayTimer -= Time.deltaTime;
                delayedFillImage.fillAmount = Mathf.Lerp(
                    _currentFillAmount, 
                    _targetFillAmount, 
                    1 - (_delayTimer / delayDuration)
                );
            }
        }

        public virtual void SetProgress(float progressPercentage)
        {
            progressPercentage = Mathf.Clamp01(progressPercentage);

            _targetFillAmount = progressPercentage;
            fillImage.fillAmount = _targetFillAmount;
            fillImage.color = progressGradient.Evaluate(_targetFillAmount);
            
            _delayTimer = delayDuration;
            _currentFillAmount = delayedFillImage.fillAmount;
        }
        
        public void Initialize()
        {
            _currentFillAmount = 1f;
            _targetFillAmount = 1f;
            fillImage.fillAmount = 1f;
            delayedFillImage.fillAmount = 1f;
            fillImage.color = progressGradient.Evaluate(1f);
        }
    }
}
