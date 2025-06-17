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
        
        // ������ɺ󷵻ؿ���״̬
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

        // ��ȡ��ǰѡ�е�����
        var trapToPlace = PlayerBag.Instance._trapBag[0];
        
        // �����������Ƴ�
        if (--trapToPlace.amount <= 0)
        {
            PlayerBag.Instance._trapBag.RemoveAt(0);
        }

        // �������λ��
        // ��Ҫ��������ϵͳ����
        Vector3 placePosition = player.transform.position + player.transform.forward * 2f;
        
        // ʵ��������Ԥ����
        TrapBase newTrap = TrapFactory.Create(trapToPlace.trapID, placePosition);
        if (newTrap != null)
        {
            newTrap.transform.rotation = player.transform.rotation;
            hasThrown = true;
        }
    }
}
