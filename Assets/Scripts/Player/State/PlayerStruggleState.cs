using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// �������״̬
    /// </summary>
    public class PlayerStruggleState : PlayerStateBase
    {
        public override void Update()
        {
            if(player.InputSettings.GetStruggleDown())
            {
                player.Struggle();
            }
        }
    }
}
