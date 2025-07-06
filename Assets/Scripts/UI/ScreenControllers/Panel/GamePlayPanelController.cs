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
    
    [SerializeField] private Transform trapHudContainer; // ���ڷ�������ͼ�������
    [SerializeField] private GameObject trapIconPrefab; // ��������ͼ���Ԥ����
    [SerializeField] private int maxTrapSlots = 4; // ��������λ����

    private List<TrapHudIcon> currentTrapIcons = new List<TrapHudIcon>(); // ��ǰ��ʾ������ͼ��
    private int selectedTrapIndex = 0; // ��ǰѡ�е���������

    private void Start()
    {
        RegisterEvents();
        
        UpdateTrapHud();
    }
    
    // ��ӿ�ݼ��л�����
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Signals.Get<GamePauseSignal>().Dispatch();
        }
        
        // ���ּ��л�����
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectTrap(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectTrap(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectTrap(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectTrap(3);
        
        // �����л�����
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

    #region �¼�����

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
    
        // ������ͼ��
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