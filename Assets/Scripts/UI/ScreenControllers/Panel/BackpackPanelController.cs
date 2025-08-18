using System;
using System.Collections.Generic;
using KidGame.UI;
using KidGame.UI.Game;
using Utils;
using UnityEngine;
using UnityEngine.UI;
using KidGame.Core;

public class ControlBackpackPanelSignal : ASignal
{
}


[Serializable]
public class BackpackPanel
{
    [SerializeField] private Sprite sprite = null;
    [SerializeField] private string buttonText = "";
    [SerializeField] private string targetScreen = "";

    public Sprite Sprite => sprite;
    public string ButtonText => buttonText;
    public string TargetScreen => targetScreen;
}

public class GotoSelectedPanel : ASignal<string>
{
}

public class BackpackPanelController : PanelController
{
    [SerializeField] private List<BackpackPanel> navigationTargets = new List<BackpackPanel>();
    [SerializeField] private NavigationPanelButton templateButton = null;

    private readonly List<NavigationPanelButton> currentButtons = new List<NavigationPanelButton>();

    // 一般来说AddListeners和RemoveListeners都是成对出现的，别add完忘记remove
    protected override void AddListeners()
    {
        Signals.Get<GotoSelectedPanel>().AddListener(OnExternalNavigation);
        Signals.Get<ControlBackpackPanelSignal>().AddListener(Control);
    }
    
    protected override void RemoveListeners()
    {
        Signals.Get<GotoSelectedPanel>().RemoveListener(OnExternalNavigation);
        Signals.Get<ControlBackpackPanelSignal>().RemoveListener(Control);
    }
    
    private void Control()
    {
        if (gameObject.activeInHierarchy)
        {
            GameManager.Instance.GameResume();
            CloseBag();
        }
        else
        {
            GameManager.Instance.GamePause();
            Show();
        }
    }
    
    /// <summary>
    /// 当界面打开时候，这个函数被调用
    /// </summary>
    protected override void OnPropertiesSet()
    {
        ClearEntries();
        
        foreach (var target in navigationTargets)
        {
            var newBtn = Instantiate(templateButton);
            newBtn.transform.SetParent(templateButton.transform.parent, false);
            newBtn.SetData(target);
            newBtn.gameObject.SetActive(true);
            newBtn.ButtonClicked += OnNavigationButtonClicked;
            currentButtons.Add(newBtn);
        }

        // 默认选中第一个按钮
        OnNavigationButtonClicked(currentButtons[0]);
        templateButton.gameObject.SetActive(false);
        UIController.Instance.uiCanvas.GetComponent<GraphicRaycaster>().enabled = true;
    }

    private void OnNavigationButtonClicked(NavigationPanelButton currentlyClickedButton)
    {
        Signals.Get<GotoSelectedPanel>().Dispatch(currentlyClickedButton.Target);
        foreach (var button in currentButtons)
        {
            button.SetCurrentNavigationTarget(currentlyClickedButton);
        }
    }

    private void OnExternalNavigation(string screenId)
    {
        foreach (var button in currentButtons)
        {
            button.SetCurrentNavigationTarget(screenId);
        }
    }

    private void ClearEntries()
    {
        foreach (var button in currentButtons)
        {
            button.ButtonClicked -= OnNavigationButtonClicked;
            Destroy(button.gameObject);
        }

        currentButtons.Clear();
    }

    public void CloseBag()
    {
        Hide();
        UIController.Instance.CloseCurrentWindow();
    }

    
}