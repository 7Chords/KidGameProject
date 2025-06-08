using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����������ͣ�����л���Ѳ�ߡ�����������������빥��
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