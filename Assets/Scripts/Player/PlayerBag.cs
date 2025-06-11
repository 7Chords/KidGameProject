using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBag : Singleton<PlayerBag>
{
    #region ��Ʒ�б�

    // ��ǰ�񵽵���Ʒ���Լ��Ӵ浵�ж�ȡ���ݡ��������ݵ��߼�

    #endregion
    
    #region ע���¼�

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