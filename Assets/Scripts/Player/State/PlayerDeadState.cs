using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// �������״̬
    /// </summary>
    public class PlayerDeadState : PlayerStateBase
    {
        public override void Enter()
        {
            player.Dead();
        }
    }
}
