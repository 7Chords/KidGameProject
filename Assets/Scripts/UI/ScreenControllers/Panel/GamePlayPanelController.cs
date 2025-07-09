using System;
using System.Collections.Generic;
using KidGame.UI;
using UnityEngine;
using Utils;
using KidGame.Core;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GamePlayPanelController : MonoBehaviour
{
    [SerializeField] private ProgressBar energyProgressBar;
    [SerializeField] private ClockProgressBar clockBar;

    #region 生命值变量

    [SerializeField] private Transform healthHudContainer;
    [SerializeField] private GameObject healthIconPrefab;
    [SerializeField] private GameObject lostHealthIconPrefab; // New prefab for lost health
    [SerializeField] private int maxhealthSlots;
    
    private List<GameObject> healthIcons = new List<GameObject>();

    #endregion
    
    #region 陷阱变量

    [SerializeField] private Transform trapHudContainer;
    [SerializeField] private GameObject trapIconPrefab;
    [SerializeField] private int maxTrapSlots = 4;

    private List<TrapHudIcon> currentTrapIcons = new List<TrapHudIcon>();
    private int selectedTrapIndex = 0;

    #endregion

    private void Start()
    {
        RegisterEvents();
        InitializeHealthHud();
        UpdateTrapHud();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectTrap(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectTrap(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectTrap(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectTrap(3);
        
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

    #region 注册事件

    private void RegisterEvents()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnHealthChanged += UpdateHealthBar;
            PlayerController.Instance.OnStaminaChanged += UpdateStaminaBar; // 新增体力变化事件监听
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
            PlayerController.Instance.OnStaminaChanged -= UpdateStaminaBar; // 移除体力变化事件监听
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

    #region 生命UI

    private void InitializeHealthHud()
    {
        foreach (var icon in healthIcons)
        {
            Destroy(icon);
        }
        healthIcons.Clear();
        
        for (int i = 0; i < maxhealthSlots; i++)
        {
            var icon = Instantiate(healthIconPrefab, healthHudContainer);
            healthIcons.Add(icon);
        }
    }

    private void UpdateHealthBar(float healthPercentage)
    {
        if (healthHudContainer == null || healthIconPrefab == null || lostHealthIconPrefab == null) 
            return;

        float maxHealth = PlayerController.Instance.MaxHealth;
        int currentHealth = Mathf.RoundToInt(maxHealth * healthPercentage);

        while (healthIcons.Count < maxHealth)
        {
            var icon = Instantiate(healthIconPrefab, healthHudContainer);
            healthIcons.Add(icon);
        }

        while (healthIcons.Count > maxHealth)
        {
            var lastIndex = healthIcons.Count - 1;
            Destroy(healthIcons[lastIndex]);
            healthIcons.RemoveAt(lastIndex);
        }
        
        for (int i = 0; i < healthIcons.Count; i++)
        {
            if (i < currentHealth)
            {
                if (healthIcons[i].gameObject != healthIconPrefab)
                {
                    Destroy(healthIcons[i]);
                    healthIcons[i] = Instantiate(healthIconPrefab, healthHudContainer);
                }
            }
            else
            {
                if (healthIcons[i].gameObject != lostHealthIconPrefab)
                {
                    Destroy(healthIcons[i]);
                    healthIcons[i] = Instantiate(lostHealthIconPrefab, healthHudContainer);
                }
            }
        }
    }

    #endregion

    #region 体力UI

    /// <summary>
    /// 更新体力进度条显示
    /// </summary>
    /// <param name="staminaPercentage">当前体力百分比</param>
    private void UpdateStaminaBar(float staminaPercentage)
    {
        if (energyProgressBar != null)
        {
            energyProgressBar.SetProgress(staminaPercentage);
        }
    }

    #endregion

    #region 选择陷阱

    private void UpdateTrapHud()
    {
        foreach (var icon in currentTrapIcons)
        {
            Destroy(icon.gameObject);
        }
        currentTrapIcons.Clear();
        
        var traps = PlayerBag.Instance.GetTrapSlots();
    
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

    #endregion
    
    private void UpdateTimeClock(GameLevelManager.LevelPhase phase, float timePercentage)
    {
        if (clockBar != null)
        {
            clockBar.UpdatePhaseTime(phase, timePercentage);
        }
    }
}