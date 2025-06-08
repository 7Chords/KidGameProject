using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBag : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerUtil.Instance.RegPlayerPickItem(PlayerInteractItem);

    }

    private void PlayerInteractItem(IPickable iPickable)
    {
        if (iPickable == null) return;
        
    }

    private void OnDisable()
    {
        
    }
}
