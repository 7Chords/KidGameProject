using System;
using KidGame.UI;
using UnityEngine;
using Utils;
using KidGame.Core;


public class GamePauseSignal : ASignal
{
}

public class GamePlayPanelController : PanelController
{
    [SerializeField] private HealthBar healthBar;
    
    private bool _isPlayerRegistered = false;

    #region 生命周期

    private void OnEnable()
    {
        TryRegisterPlayerHealth();
    }

    private void OnDisable()
    {
        UnregisterPlayerHealth();
    }

    private void Start()
    {
        if (healthBar != null)
        {
            healthBar.Initialize();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Signals.Get<GamePauseSignal>().Dispatch();
        }
        
        if (!_isPlayerRegistered)
        {
            TryRegisterPlayerHealth();
        }
    }    

    #endregion
    
    #region 注册体力条

    private void TryRegisterPlayerHealth()
    {
        if (_isPlayerRegistered) return;
        
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnHealthChanged += UpdateHealthBar;
            _isPlayerRegistered = true;
            
            if (healthBar != null)
            {
                float healthPercentage = PlayerController.Instance.CurrentHealth / PlayerController.Instance.MaxHealth;
                healthBar.SetHealth(healthPercentage);
            }
        }
    }
    
    private void UnregisterPlayerHealth()
    {
        if (_isPlayerRegistered && PlayerController.Instance != null)
        {
            PlayerController.Instance.OnHealthChanged -= UpdateHealthBar;
            _isPlayerRegistered = false;
        }
    }    

    #endregion
    
    private void UpdateHealthBar(float healthPercentage)
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(healthPercentage);
        }
    }
}