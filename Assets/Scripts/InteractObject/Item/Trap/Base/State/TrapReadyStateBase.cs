using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class TrapReadyStateBase : TrapStateBase
    {

        public override void Enter()
        {
            trap.ReadyIndicator.material.SetColor("_Color", trap.ReadyColor);
        }
    }
}
