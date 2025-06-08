using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���е���״̬�Ļ���
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
