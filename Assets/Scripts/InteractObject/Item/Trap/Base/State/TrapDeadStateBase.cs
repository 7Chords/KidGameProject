using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class TrapDeadStateBase : TrapStateBase
    {
        public override void Enter()
        {
            trap.Discard();
        }




    }
}
