using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public override void Enter()
    {
        //player.PlayAnimation("Idle");

    }

    public override void Update()
    {
        player.CharacterController.Move(new Vector3(0, -9.8f * Time.deltaTime, 0));
        // �����ҵ�����
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            // �л�״̬
            player.ChangeState(PlayerState.Move);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
