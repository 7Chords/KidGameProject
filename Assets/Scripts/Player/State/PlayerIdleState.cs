using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI;
using UnityEngine;

namespace KidGame.Core
{
    public class PlayerIdleState : PlayerStateBase
    {
        public override void Enter()
        {
            player.PlayAnimation("Idle");
        }

        public override void Update()
        {
            base.Update();
            //player.Rotate();
            Vector2 inputVal = player.InputSettings.MoveDir();
            // �����ҵ�����
            if (inputVal != Vector2.zero)
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
}