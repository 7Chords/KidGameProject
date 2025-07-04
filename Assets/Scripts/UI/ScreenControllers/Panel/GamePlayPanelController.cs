using System;
using KidGame.UI;
using UnityEngine;
using Utils;
using KidGame.Core;

public class GamePauseSignal : ASignal
{
}

public class GamePlayPanelController : MonoBehaviour
{
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private ClockProgressBar clockBar;

    private void Start()
    {
        RegisterEvents();
    }
    
    private void OnDestroy()
    {
        UnregisterAllEvents();
    }

    #region 事件管理

    private void RegisterEvents()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnHealthChanged += UpdateHealthBar;
        }
        
        if (GameLevelManager.Instance != null)
        {
            GameLevelManager.Instance.OnPhaseTimeUpdated += UpdateTimeClock;
        }
    }

    private void UnregisterAllEvents()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnHealthChanged -= UpdateHealthBar;
        }
        
        if (GameLevelManager.Instance != null)
        {
            GameLevelManager.Instance.OnPhaseTimeUpdated -= UpdateTimeClock;
        }
    }

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Signals.Get<GamePauseSignal>().Dispatch();
        }
    }
    
    private void UpdateHealthBar(float healthPercentage)
    {
        if (progressBar != null)
        {
            progressBar.SetProgress(healthPercentage);
        }
    }

    private void UpdateTimeClock(GameLevelManager.LevelPhase phase, float timePercentage)
    {
        if (clockBar != null)
        {
            clockBar.UpdatePhaseTime(phase, timePercentage);
        }
    }
}