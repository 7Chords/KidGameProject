using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;

namespace KidGame.Core
{
    public class PlayerMoveState : PlayerStateBase
    {
        private Vector3 dir;
        private float speed;
        private bool isRunning;

        public override void Enter()
        {
            //player.PlayerAnimation("Move");
            isRunning = player.InputSettings.GetIfRun();
            UpdateSpeed();
        }

        public override void Update()
        {
            base.Update();
            
            // 冲刺
            if (player.InputSettings.GetDashDown())
            {
                player.ChangeState(PlayerState.Dash);
                return;
            }

            // 奔跑状态切换
            bool runningInput = player.InputSettings.GetIfRun();
            if (runningInput != isRunning)
            {
                isRunning = runningInput;
                UpdateSpeed();
            }
            if(isRunning)
            {
                player.ConsumeStamina(GameModel.Instance.PlayerInfo.RunStaminaPerSecond * Time.deltaTime);
                //发出逻辑上的声音
                player.ProduceSound(GameModel.Instance.PlayerInfo.RunSoundRange);
            }
            Vector3 move = player.InputSettings.CameraRelativeMoveDir(CameraController.Instance.transform);

            if (move == Vector3.zero)
            {
                player.ChangeState(PlayerState.Idle);
            }
            else
            {
                dir = move;  // 相机相对的方向
            }

            player.Rotate();

        }

        private void UpdateSpeed()
        {
            speed = isRunning ? GameModel.Instance.PlayerInfo.RunSpeed : GameModel.Instance.PlayerInfo.WalkSpeed;
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
}