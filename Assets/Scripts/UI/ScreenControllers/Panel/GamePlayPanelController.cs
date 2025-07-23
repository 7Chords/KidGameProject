using System;
using System.Collections.Generic;
using KidGame.UI;
using UnityEngine;
using Utils;
using KidGame.Core;
using UnityEngine.Serialization;
using UnityEngine.UI;
using KidGame;

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

    private List<ItemHudIcon> currentItemIcons = new List<ItemHudIcon>();
    private int selectedItemIndex = 0;

    #endregion

    #region ��������
    public Text ScoreText;
    #endregion

    public void Init()
    {
        RegisterEvents();
        InitializeHealthHud();
        InitializeQuickAccessHud();
    }
    
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
        
        PlayerBag.Instance.OnQuickAccessBagUpdated += UpdateQuickAccessHud;
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
            PlayerBag.Instance.OnQuickAccessBagUpdated -= UpdateQuickAccessHud;
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
            int newIndex = selectedItemIndex + direction;
            newIndex = (newIndex + GlobalValue.QUICK_ACCESS_BAG_CAPACITY) % GlobalValue.QUICK_ACCESS_BAG_CAPACITY;
            SelectTrap(newIndex);
        }
    }

    private void InitializeQuickAccessHud()
    {
        for (int i = 0; i < GlobalValue.QUICK_ACCESS_BAG_CAPACITY; i++)
        {
            var iconObj = Instantiate(trapIconPrefab, trapHudContainer);
            var icon = iconObj.GetComponent<ItemHudIcon>();
            icon.SetEmpty();
            currentItemIcons.Add(icon);

            icon.SetSelected(i == selectedItemIndex);

            int index = i;
            //ע���¼�
            iconObj.GetComponent<Button>().onClick.AddListener(() => SelectTrap(index));
        }
        UpdateQuickAccessHud();
    }
    private void UpdateQuickAccessHud()
    {
        
        var traps = PlayerBag.Instance.GetQuickAccessBag();

        for (int i = 0; i < GlobalValue.QUICK_ACCESS_BAG_CAPACITY; i++)
        {
            var icon = currentItemIcons[i];
            if (i< traps.Count)
            {
                icon.Setup(traps[i]);
            }
            else
            {
                //��Ҫ������ ������Ӧ���ǳ�פ�� û����Ҳ��Ҫɾ��
                icon.SetEmpty();
            }
            icon.SetSelected(i == selectedItemIndex);
        }
    }

    private void SelectTrap(int index)
    {
        if (index < 0 || index >= currentItemIcons.Count) return;

        for (int i = 0; i < currentItemIcons.Count; i++)
        {
            currentItemIcons[i].SetSelected(i == index);
        }

        selectedItemIndex = index;
        PlayerBag.Instance.SelectedIndex = index;
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