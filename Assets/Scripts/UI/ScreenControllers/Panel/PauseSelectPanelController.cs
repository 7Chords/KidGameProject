using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.UI;
using UnityEngine;
using UnityEngine.UI;


public class PauseSelectPanelController : PanelController
{
    [SerializeField] private Toggle backpagToggle;
    [SerializeField] private Toggle handBookToggle;
    [SerializeField] private Toggle settingToggle;
    
    
    /// <summary>
    /// 当界面打开时候，这个函数被调用
    /// </summary>
    protected override void OnPropertiesSet()
    {
        backpagToggle.isOn = true;
        UI_ShowBackpackWindow(true);
        backpagToggle.onValueChanged.AddListener((isOn) =>
        {
            UI_ShowBackpackWindow(isOn);
        });
    }
    private void UI_ShowBackpackWindow(bool isShow)
    {
        
    }
}
