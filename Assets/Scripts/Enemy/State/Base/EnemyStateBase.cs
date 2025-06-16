using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 所有敌人状态的基类
    /// </summary>
    public class EnemyStateBase : StateBase
    {
        protected EnemyController enemy;

        public override void Init(IStateMachineOwner owner, int stateType, StateMachine stateMachine)
        {
            base.Init(owner, stateType, stateMachine);
            enemy = (EnemyController)owner;
        }
    }
}