using System;
using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UIFramework.Examples;
using UnityEngine;
using Utils;


public class SettingWindowController : WindowController
{
    private bool isSaved = false;
    protected override void Awake() {
        base.Awake();
        isSaved = false;
    }
    public void UI_Save()
    {
        isSaved = true;
        
        //TODO
        //这里放保存设置的方法
        
    }
    public void UI_ShowPopup() {
        if(!isSaved)
            Signals.Get<ShowConfirmationPopupSignal>().Dispatch(GetPopupData());
        else
            UI_Close();
    }
    private ConfirmationPopupProperties GetPopupData()
    {
        ConfirmationPopupProperties testProps = null;
        testProps = new ConfirmationPopupProperties("确认设置", 
            "现在退出设置会失效，确认要退出吗",
            "确认",UI_Close,
            "取消",DoNothing
            );
        
        return testProps;
        
    }

    private void DoNothing()
    {
        
    }
    
}
