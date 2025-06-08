using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 待机，稍作停留后切换到巡逻。若发现玩家立即进入攻击
/// </summary>
public class EnemyIdleState : EnemyStateBase
{
    private float _timer;
    private const float _idleDuration = 1.5f;

    public override void Enter()
    {
        _timer = 0f;
        // enemy.PlayAnimation("Idle");
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (enemy.PlayerInSight() || enemy.PlayerInHearing())
        {
            enemy.ChangeState(EnemyState.Attack);
            return;
        }

        if (_timer >= _idleDuration)
        {
            enemy.ChangeState(EnemyState.Patrol);
        }
    }
}