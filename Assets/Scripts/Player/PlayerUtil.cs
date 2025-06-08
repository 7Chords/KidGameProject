using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����¼�����
/// </summary>
public class PlayerUtil : Singleton<PlayerUtil>
{
    private InputSettings inputSettings;

    //��Ұ��½�����ť���¼�
    private Action onPlayerInteractPressed;
    public Action OnPlayerInteractPressed { get { return onPlayerInteractPressed; } set { onPlayerInteractPressed = value; } }

    //��Ҽ񵽳�����Ʒ������͵��ߣ����¼�
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
