using System;
using System.Collections.Generic;
using KidGame.UI;
using UnityEngine;
using Utils;
using KidGame.Core;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GamePlayPanelController : Singleton<GamePlayPanelController>
{
    [SerializeField] private ProgressBar energyProgressBar;
    [SerializeField] private ClockProgressBar clockBar;

    #region ����ֵ����

    [SerializeField] private Transform healthHudContainer;
    [SerializeField] private GameObject healthIconPrefab;
    [SerializeField] private GameObject lostHealthIconPrefab; // New prefab for lost health
    [SerializeField] private int maxhealthSlots;
    
    private List<GameObject> healthIcons = new List<GameObject>();

    #endregion
    
    #region �������

    [SerializeField] private Transform trapHudContainer;
    [SerializeField] private GameObject trapIconPrefab;
    [SerializeField] private int maxTrapSlots = 4;

    private List<TrapHudIcon> currentTrapIcons = new List<TrapHudIcon>();
    private int selectedTrapIndex = 0;

    #endregion

    #region ��������
    public Text ScoreText;
    #endregion

    public void Init()
    {
        RegisterEvents();
        InitializeHealthHud();
        UpdateTrapHud();
    }
    
    //private void Update()
    //{
    //    float scroll = Input.GetAxis("Mouse ScrollWheel");
    //    if (scroll != 0)
    //    {
    //        int direction = scroll > 0 ? -1 : 1;
    //        int newIndex = selectedTrapIndex + direction;
    //        int trapCount = PlayerBag.Instance.GetTempTrapSlots().Count;
    //        if (trapCount > 0)
    //        {
    //            newIndex = (newIndex + trapCount) % trapCount;
    //            SelectTrap(newIndex);
    //        }
    //    }
    //}
    
    public void Discard()
    {
        UnregisterAllEvents();
    }

    #region ע���¼�

    private void RegisterEvents()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnHealthChanged += UpdateHealthBar;
            PlayerController.Instance.OnStaminaChanged += UpdateStaminaBar; // ���������仯�¼�����
            PlayerController.Instance.OnMouseWheelValueChanged += UpdateSelectItem;
        }
        
        if (GameLevelManager.Instance != null)
        {
            GameLevelManager.Instance.OnPhaseTimeUpdated += UpdateTimeClock;
        }
        
        PlayerBag.Instance.OnTrapBagUpdated += UpdateTrapHud;
        GameManager.Instance.OnCurrentLoopScoreChanged += CurrentLoopScoreChanged;
        
    }


    private void UnregisterAllEvents()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnHealthChanged -= UpdateHealthBar;
            PlayerController.Instance.OnStaminaChanged -= UpdateStaminaBar; // �Ƴ������仯�¼�����
            PlayerController.Instance.OnMouseWheelValueChanged -= UpdateSelectItem;

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

    #region ����UI

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

    #region ����UI

    /// <summary>
    /// ����������������ʾ
    /// </summary>
    /// <param name="staminaPercentage">��ǰ�����ٷֱ�</param>
    private void UpdateStaminaBar(float staminaPercentage)
    {
        if (energyProgressBar != null)
        {
            energyProgressBar.SetProgress(staminaPercentage);
        }
    }

    #endregion

    #region ѡ������


    private void UpdateSelectItem(float scrollValue)
    {
        if (scrollValue != 0)
        {
            int direction = scrollValue > 0 ? 1 : -1;
            int newIndex = selectedTrapIndex + direction;
            int trapCount = PlayerBag.Instance.GetTempTrapSlots().Count;
            if (trapCount > 0)
            {
                newIndex = (newIndex + trapCount) % trapCount;
                SelectTrap(newIndex);
            }
        }
    }
    private void UpdateTrapHud()
    {
        foreach (var icon in currentTrapIcons)
        {
            Destroy(icon.gameObject);
        }

        currentTrapIcons.Clear();

        var traps = PlayerBag.Instance.GetTempTrapSlots();

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

    #region ����UI
    private void CurrentLoopScoreChanged(int score)
    {
        ScoreText.text = score.ToString();
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