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
            //todo
            player.Rb.AddForce(player.transform.forward * 100f, ForceMode.Impulse);
            player.ConsumeStamina(player.PlayerBaseData.DashStaminaOneTime);
        }
    }
}