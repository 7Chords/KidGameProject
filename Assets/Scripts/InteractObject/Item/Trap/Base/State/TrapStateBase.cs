using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// ����״̬����
    /// </summary>
    public class TrapStateBase : StateBase
    {
        protected TrapBase trap;//״̬��owner
        public override void Init(IStateMachineOwner owner, int stateType, StateMachine stateMachine)
        {
            base.Init(owner, stateType, stateMachine);
            trap = (TrapBase)owner;
        }

    }
}
