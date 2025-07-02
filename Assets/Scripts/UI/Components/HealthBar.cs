using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class HealthBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image fillImage;
        [SerializeField] private Image delayedFillImage;
        [SerializeField] private Gradient healthGradient;
        
        [Header("Settings")]
        [SerializeField] private float delayDuration = 0.5f;
        
        private float _currentFillAmount = 1f;
        private float _targetFillAmount = 1f;
        private float _delayTimer = 0f;
        
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

        public void SetHealth(float healthPercentage)
        {
            healthPercentage = Mathf.Clamp01(healthPercentage);

            _targetFillAmount = healthPercentage;
            fillImage.fillAmount = _targetFillAmount;
            fillImage.color = healthGradient.Evaluate(_targetFillAmount);
            
            _delayTimer = delayDuration;
            _currentFillAmount = delayedFillImage.fillAmount;
        }
        
        public void Initialize()
        {
            _currentFillAmount = 1f;
            _targetFillAmount = 1f;
            fillImage.fillAmount = 1f;
            delayedFillImage.fillAmount = 1f;
            fillImage.color = healthGradient.Evaluate(1f);
        }
    }
}
