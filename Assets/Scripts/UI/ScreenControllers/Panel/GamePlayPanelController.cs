using System;
using System.Collections.Generic;
using KidGame.UI;
using UnityEngine;
using Utils;
using KidGame.Core;
using UnityEngine.UI;

public class GamePauseSignal : ASignal
{
}

public class GamePlayPanelController : MonoBehaviour
{
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private ClockProgressBar clockBar;
    
    [SerializeField] private Transform trapHudContainer; // 用于放置陷阱图标的容器
    [SerializeField] private GameObject trapIconPrefab; // 单个陷阱图标的预制体
    [SerializeField] private int maxTrapSlots = 4; // 最大陷阱槽位数量

    private List<TrapHudIcon> currentTrapIcons = new List<TrapHudIcon>(); // 当前显示的陷阱图标
    private int selectedTrapIndex = 0; // 当前选中的陷阱索引

    private void Start()
    {
        RegisterEvents();
        
        UpdateTrapHud();
    }
    
    // 添加快捷键切换陷阱
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Signals.Get<GamePauseSignal>().Dispatch();
        }
        
        // 数字键切换陷阱
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectTrap(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectTrap(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectTrap(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectTrap(3);
        
        // 滚轮切换陷阱
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int direction = scroll > 0 ? -1 : 1;
            int newIndex = PlayerBag.Instance.SelectedTrapIndex + direction;
            int trapCount = PlayerBag.Instance.GetTrapSlots().Count;
            if (trapCount > 0)
            {
                newIndex = (newIndex + trapCount) % trapCount;
                SelectTrap(newIndex);
            }
        }
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
        
        PlayerBag.Instance.OnTrapBagUpdated += UpdateTrapHud;
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
        
        if (PlayerBag.Instance != null)
        {
            PlayerBag.Instance.OnTrapBagUpdated -= UpdateTrapHud;
        }
    }

    #endregion
    
    private void UpdateTrapHud()
    {
        foreach (var icon in currentTrapIcons)
        {
            Destroy(icon.gameObject);
        }
        currentTrapIcons.Clear();
        
        var traps = PlayerBag.Instance.GetTrapSlots();
    
        // 创建新图标
        for (int i = 0; i < Mathf.Min(traps.Count, maxTrapSlots); i++)
        {
            var iconObj = Instantiate(trapIconPrefab, trapHudContainer);
            var icon = iconObj.GetComponent<TrapHudIcon>();
            icon.Setup(traps[i], i == selectedTrapIndex);
            currentTrapIcons.Add(icon);
            
            int index = i;
            iconObj.GetComponent<Button>().onClick.AddListener(() => SelectTrap(index));
        }
    }

    private void SelectTrap(int index)
    {
        if (index < 0 || index >= currentTrapIcons.Count) return;
        
        for (int i = 0; i < currentTrapIcons.Count; i++)
        {
            currentTrapIcons[i].SetSelected(i == index);
        }
    
        selectedTrapIndex = index;
        PlayerBag.Instance.SelectedTrapIndex = index;
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