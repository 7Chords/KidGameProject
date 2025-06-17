using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KidGame.Core;


public class PlayerThrowState : PlayerStateBase
{
    private float throwTimer;
    private bool hasThrown;
    
    public override void Enter()
    {
        //player.PlayerAnimation("Throw");
        throwTimer = 0f;
        hasThrown = false;
        
        TryPlaceTrap();
    }

    public override void Update()
    {
        base.Update();
        
        // 放置完成后返回空闲状态
        throwTimer += Time.deltaTime;
        if (throwTimer > 0.5f && hasThrown)
        {
            player.ChangeState(PlayerState.Idle);
        }
    }

    private void TryPlaceTrap()
    {
        if (PlayerBag.Instance._trapBag.Count == 0)
        {
            hasThrown = true;
            return;
        }

        // 获取当前选中的陷阱
        var trapToPlace = PlayerBag.Instance._trapBag[0];
        
        // 减少数量或移除
        if (--trapToPlace.amount <= 0)
        {
            PlayerBag.Instance._trapBag.RemoveAt(0);
        }

        // 计算放置位置
        // 需要根据网格系统调整
        Vector3 placePosition = player.transform.position + player.transform.forward * 2f;
        
        // 实例化陷阱预制体
        TrapBase newTrap = TrapFactory.Create(trapToPlace.trapID, placePosition);
        if (newTrap != null)
        {
            newTrap.transform.rotation = player.transform.rotation;
            hasThrown = true;
        }
    }
}
