using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class PlayerUseState : PlayerStateBase
    {
        private float useTimer;
        private bool hasPlaced;

        public override void Enter()
        {
            //player.PlayerAnimation("Throw");
            useTimer = 0f;
            hasPlaced = false;

            TryPlaceTrap();
        }

        public override void Update()
        {
            base.Update();

            // 放置完成后返回空闲状态
            useTimer += Time.deltaTime;
            if (useTimer > 0.5f && hasPlaced)
            {
                player.ChangeState(PlayerState.Idle);
            }
        }

        private void TryPlaceTrap()
        {
            if (PlayerBag.Instance._trapBag.Count == 0)
            {
                hasPlaced = true;
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
            Vector3 placePosition = player.transform.position + player.transform.forward + Vector3.up;

            // 实例化陷阱预制体
            GameObject newTrap = TrapFactory.Create(trapToPlace.trapData, placePosition);
            if (newTrap != null)
            {
                newTrap.transform.rotation = player.transform.rotation;
                hasPlaced = true;
            }
        }
    }
}
