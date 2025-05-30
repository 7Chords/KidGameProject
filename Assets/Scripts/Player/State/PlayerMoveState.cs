using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    private Vector3 dir;
    private float speed;
    public override void Enter()
    {
        //player.PlayerAnimation("Move");
    }

    public override void Update()
    {
        player.Rotate();
        if (player.InputSettings.GetDashDown())
        {
            // ÇÐ»»×´Ì¬
            player.ChangeState(PlayerState.Dash);
        }
        Vector2 inputVal = player.InputSettings.MoveDir();
        if (inputVal == Vector2.zero)
        {
            // ÇÐ»»×´Ì¬
            player.ChangeState(PlayerState.Idle);
        }
        else
        {
            dir = new Vector3(inputVal.x,0,inputVal.y);
            if(player.InputSettings.GetIfRun())
            {
                speed = player.PlayerBaseData.RunSpeed;
            }
            else
            {
                speed = player.PlayerBaseData.WalkSpeed;
            }
        }
    }

    public override void FixedUpdate()
    {
        player.Rb.velocity = dir * speed;

    }


    public override void Exit()
    {
        base.Exit();
    }
}
