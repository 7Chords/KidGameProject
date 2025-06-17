using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class PlayerDashState : PlayerStateBase
    {
        public override void Enter()
        {
            //player.PlayerAnimation("Dash");
            player.Rb.AddForce(player.transform.forward * 5f, ForceMode.Impulse);
            //TODO:¡Ÿ ±≤‚ ‘
            MonoManager.Instance.Invoke("ChangeToIdle", 1);
        }

        private void ChangeToIdle()
        {
            Debug.Log("ChangeToIdleChangeToIdle");
            player.ChangeState(PlayerState.Idle);
        }


    }
}
