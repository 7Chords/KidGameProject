using KidGame.Interface;
using KidGame.UI;
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
            useTimer = 0f;
            hasPlaced = false;
            
            Vector3 placePosition = player.transform.position + player.transform.forward + Vector3.up;
            Quaternion rotation = player.transform.rotation;
            
            // 调用PlayerBag的方法使用陷阱
            hasPlaced = PlayerBag.Instance.TryUseSelectedTrap(player, placePosition, rotation);
            
            if (!hasPlaced)
            {
                // 如果使用失败，立即返回空闲状态
                player.ChangeState(PlayerState.Idle);
            }
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
    }
}