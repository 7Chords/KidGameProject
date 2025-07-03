using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// ÏÝÚå×´Ì¬»ùÀà
    /// </summary>
    public class TrapStateBase : StateBase
    {
        protected TrapBase trap;//×´Ì¬»úowner
        public override void Init(IStateMachineOwner owner, int stateType, StateMachine stateMachine)
        {
            base.Init(owner, stateType, stateMachine);
            trap = (TrapBase)owner;
        }

    }
}
