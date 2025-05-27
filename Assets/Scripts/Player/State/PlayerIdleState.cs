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
        Vector2 inputVal = player.InputSettings.MoveDir();
        // ¼ì²âÍæ¼ÒµÄÊäÈë
        if (inputVal!= Vector2.zero)
        {
            // ÇÐ»»×´Ì¬
            player.ChangeState(PlayerState.Move);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
