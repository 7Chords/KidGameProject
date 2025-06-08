using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBag : Singleton<PlayerBag>
{

    public void Init()
    {
        PlayerUtil.Instance.RegPlayerPickItem(PlayerInteractItem);
    }
    public void Discard()
    {
        PlayerUtil.Instance.UnregPlayerPickItem(PlayerInteractItem);
    }
    private void PlayerInteractItem(IPickable iPickable)
    {
        if (iPickable == null) return;
        if(iPickable is MapItem)
        {
            Debug.Log($"ºÒµΩ¡À{((MapItem)iPickable).itemName}£°");
        }
    }
}
