using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI;
using KidGame.UI.Game;
using UnityEngine;
using UnityEngine.UI;
using Utils;


public class SettingWindowController : WindowController
{
    private bool isSaved = false;

    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;


    protected override void Awake()
    {
        base.Awake();
        isSaved = false;
    }

    private void Start()
    {
        mainSlider.onValueChanged.AddListener(OnMainVolumeChanged);
        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    protected override void OnPropertiesSet()
    {
        mainSlider.value = AudioManager.Instance.mainVolumeFactor;
        bgmSlider.value = AudioManager.Instance.bgmVolumeFactor;
        sfxSlider.value = AudioManager.Instance.sfxVolumeFactor;
    }

    public void UI_Save()
    {
        isSaved = true;

        // 这个audioManager是会自动保存音量参数到玩家注册表的
        // 这里没有具体的保存逻辑也可以（应该
    }

    public void UI_ShowPopup()
    {
        if (!isSaved)
            Signals.Get<ShowConfirmationPopupSignal>().Dispatch(GetPopupData());
        else
            UI_Close();
    }

    #region 改变音量

    private void OnMainVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeMainVolume(value);
    }

    private void OnBgmVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeBgmVolume(value);
    }

    private void OnSfxVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeSfxVolume(value);
    }

    #endregion

    private ConfirmationPopupProperties GetPopupData()
    {
        // 没有点击确认的话就会弹出这个会话框
        ConfirmationPopupProperties testProps = null;
        testProps = new ConfirmationPopupProperties("确认设置",
            "现在退出设置会失效，确认要退出吗",
            "确认", UI_Close,
            "取消", DoNothing
        );

        return testProps;
    }

    private void DoNothing()
    {
    }
}