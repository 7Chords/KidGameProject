using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerStateBase
{
    public override void Enter()
    {
        //player.PlayerAnimation("Dash");
        player.Rb.AddForce(player.transform.forward*5f, ForceMode.Impulse);
    }
}
