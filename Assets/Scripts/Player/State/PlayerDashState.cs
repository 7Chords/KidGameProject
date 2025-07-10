using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class PlayerDashState : PlayerStateBase
    {
        public override void Enter()
        {
            player.PlayAnimation("Dash");
            player.ProduceSound(GlobalValue.MIDDLE_RANGE_SOUND_SPREAD);
            player.Rb.AddForce(player.transform.forward * 10f, ForceMode.Impulse);
            // todo.ÏûºÄÌåÁ¦
            player.ConsumeStamina(10f);
        }
    }
}