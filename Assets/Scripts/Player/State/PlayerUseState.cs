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
            
            hasPlaced = PlayerBag.Instance.TryUseSelectedTrap(player, placePosition, rotation);
            
            if (!hasPlaced)
            {
                player.ChangeState(PlayerState.Idle);
            }
        }

        public override void Update()
        {
            base.Update();
            
            useTimer += Time.deltaTime;
            if (useTimer > 0.5f && hasPlaced)
            {
                player.ChangeState(PlayerState.Idle);
            }
        }
    }
}