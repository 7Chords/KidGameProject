using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    public override void Enter()
    {
        //player.PlayerAnimation("Move");
    }

    public override void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h == 0 && v == 0)
        {
            // �л�״̬
            player.ChangeState(PlayerState.Idle);
        }
        else
        {
            // �����ƶ�
            Vector3 input = new Vector3(h, 0, v);
            float speed = player.PlayerBaseData.MoveSpeed;
            Vector3 motion = Time.deltaTime * speed * input;
            motion.y = -9.8f * Time.deltaTime;
            characterController.Move(motion);
            //��ת
            player.Rotate(input);
        }
    }

    public override void Exit()
    {
        
    }
}
