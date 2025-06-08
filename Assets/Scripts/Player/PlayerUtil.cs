using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家事件中心
/// </summary>
public class PlayerUtil : Singleton<PlayerUtil>
{
    private InputSettings inputSettings;

    //玩家按下交互按钮的事件
    private Action onPlayerInteractPressed;
    public Action OnPlayerInteractPressed { get { return onPlayerInteractPressed; } set { onPlayerInteractPressed = value; } }

    //玩家捡到场景物品（陷阱和道具）的事件
    private Action<IPickable> onPlayerPickItem;

    protected override void Awake()
    {
        base.Awake();
        inputSettings = GetComponent<InputSettings>();
    }

    public void Init()
    {
        inputSettings.OnInteractionPress += OnPlayerInteractPressed;
    }

    public void Discard()
    {
        inputSettings.OnInteractionPress -= OnPlayerInteractPressed;
    }

    #region Call
    public void CallPlayerInteractPressed()
    {
        OnPlayerInteractPressed?.Invoke();
    }

    public void CallPlayerPickItem(IPickable iPickable)
    {
        onPlayerPickItem?.Invoke(iPickable);
    }

    #endregion

    public void RegPlayerPickItem(Action<IPickable> onPlayerPickItem)
    {
        this.onPlayerPickItem += onPlayerPickItem;
    }

    public void UnregPlayerPickItem(Action<IPickable> onPlayerPickItem)
    {
        this.onPlayerPickItem -= onPlayerPickItem;
    }



}
