using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBag : Singleton<PlayerBag>
{
    #region 物品列表

    // 当前捡到的物品、以及从存档中读取数据、保存数据的逻辑

    #endregion
    
    #region 注册事件

    public void Init()
    {
        PlayerUtil.Instance.RegPlayerPickItem(PlayerInteractItem);
    }

    public void Discard()
    {
        PlayerUtil.Instance.UnregPlayerPickItem(PlayerInteractItem);
    }

    #endregion


    private void PlayerInteractItem(IPickable iPickable)
    {
        if (iPickable == null) return;
        if (iPickable is MapItem)
        {

        }
    }
}