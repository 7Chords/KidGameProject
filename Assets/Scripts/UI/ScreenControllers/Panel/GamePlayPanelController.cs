using KidGame;
using KidGame.Core;
using KidGame.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayPanelController : Singleton<GamePlayPanelController>
{
    [SerializeField] private ProgressBar energyProgressBar;
    [SerializeField] private ClockProgressBar clockBar;

    #region 生命值变量

    [SerializeField] private Transform healthHudContainer;
    [SerializeField] private GameObject healthIconPrefab;
    [SerializeField] private GameObject lostHealthIconPrefab;
    
    private List<GameObject> healthIcons = new List<GameObject>();

    #endregion
    
    #region 陷阱变量

    [SerializeField] private Transform trapHudContainer;
    [SerializeField] private GameObject trapIconPrefab;

    private List<ItemHudIcon> currentItemIcons = new List<ItemHudIcon>();
    private int selectedItemIndex = 0;

    #endregion

    #region 分数变量
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

    #region 注册事件

    private void RegisterEvents()
    {

        MsgCenter.RegisterMsg(MsgConst.ON_HEALTH_CHG, UpdateHealthBar);
        MsgCenter.RegisterMsg(MsgConst.ON_STAMINA_CHG, UpdateStaminaBar);
        MsgCenter.RegisterMsg(MsgConst.ON_MOUSEWHEEL_VALUE_CHG, UpdateSelectItem);
        MsgCenter.RegisterMsg(MsgConst.ON_PHASE_TIME_UPDATE, UpdateTimeClock);
        MsgCenter.RegisterMsgAct(MsgConst.ON_QUICK_BAG_UPDATE, UpdateQuickAccessHud);
        MsgCenter.RegisterMsg(MsgConst.ON_CUR_LOOP_SCORE_CHG, CurrentLoopScoreChanged);
    }


    private void UnregisterAllEvents()
    {
        MsgCenter.UnregisterMsg(MsgConst.ON_HEALTH_CHG, UpdateHealthBar);
        MsgCenter.UnregisterMsg(MsgConst.ON_STAMINA_CHG, UpdateStaminaBar);
        MsgCenter.UnregisterMsg(MsgConst.ON_MOUSEWHEEL_VALUE_CHG, UpdateSelectItem);
        MsgCenter.UnregisterMsg(MsgConst.ON_PHASE_TIME_UPDATE, UpdateTimeClock);
        MsgCenter.UnregisterMsgAct(MsgConst.ON_QUICK_BAG_UPDATE, UpdateQuickAccessHud);
        MsgCenter.UnregisterMsg(MsgConst.ON_CUR_LOOP_SCORE_CHG, CurrentLoopScoreChanged);
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
        
        for (int i = 0; i < PlayerController.Instance.MaxHealth; i++)
        {
            var icon = Instantiate(healthIconPrefab, healthHudContainer);
            healthIcons.Add(icon);
        }
    }

    private void UpdateHealthBar(object[] objs)
    {
        if (objs == null || objs.Length == 0) return;
        int currentHealth = (int)objs[0];
        if (healthHudContainer == null || healthIconPrefab == null || lostHealthIconPrefab == null) 
            return;

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

    private void UpdateStaminaBar(object[] objs)
    {
        if (objs == null || objs.Length == 0) return;
        float staminaPercentage = (float)objs[0];
        if (energyProgressBar != null)
        {
            energyProgressBar.SetProgress(staminaPercentage);
        }
    }

    #endregion

    #region 选择陷阱


    private void UpdateSelectItem(object[] objs)
    {
        if (objs == null || objs.Length == 0) return;
        float scrollValue = (float)objs[0];
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
            //注册事件
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
                //不要销毁啦 道具栏应该是常驻的 没东西也不要删掉
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

    #region 分数UI

    private void CurrentLoopScoreChanged(object[] objs)
    {
        if (objs == null || objs.Length == 0) return;
        int score = (int)objs[0];
        ScoreText.text = score.ToString();
    }

    #endregion

    #region 时间UI

    private void UpdateTimeClock(object[] objs)
    {
        if (objs == null || objs.Length == 0) return;
        LevelPhase phase = (LevelPhase)objs[0];
        float timePercentage = (float)objs[1];
        if (clockBar != null)
        {
            clockBar.UpdatePhaseTime(phase, timePercentage);
        }
    }

    #endregion
}