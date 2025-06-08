using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ѳ�ߣ���·��������ߡ������ϰ����֪���ʱ����Ӧ����
/// </summary>
public class EnemyPatrolState : EnemyStateBase
{
    private Vector3 _targetPoint;
    
    private const float CheckDistance = 1.5f;
    private const float CheckRadius   = 0.25f;
    private const float MinTargetThreshold = 0.2f;

    public override void Enter()
    {
        enemy.PatrolTimer = 0f;
        PickNextPoint();
        // enemy.PlayAnimation("Walk");
    }

    public override void Update()
    {
        // ������
        if (enemy.PlayerInSight())
        {
            Debug.Log("AttackPlayer");
            enemy.ChangeState(EnemyState.Attack);
            return;
        }

        // ����Ѳ�ߵ�
        if (enemy.PatrolPoints == null || enemy.PatrolPoints.Length == 0)
        {
            enemy.ChangeState(EnemyState.Idle);
            return;
        }

        // ���·�����赲����Ŀ��
        if (PathBlocked())
        {
            PickNextPoint();
            return;
        }

        // �����ƶ�
        Vector3 dir = (_targetPoint - enemy.transform.position).normalized;
        enemy.Rb.velocity = dir * enemy.EnemyBaseData.MoveSpeed;
        
        if (Vector3.Distance(enemy.transform.position, _targetPoint) < MinTargetThreshold)
        {
            enemy.Rb.velocity = Vector3.zero;
            enemy.PatrolTimer += Time.deltaTime;
            if (enemy.PatrolTimer >= enemy.PatrolWaitTime)
            {
                enemy.PatrolTimer = 0f;
                PickNextPoint();
            }
        }
    }

    public override void Exit() => enemy.Rb.velocity = Vector3.zero;

    /// <summary>
    /// ѡȡ��һѲ�ߵ㣬˳��ѭ�����������赲�����Զ��
    /// </summary>
    private void PickNextPoint()
    {
        if (enemy.PatrolPoints == null || enemy.PatrolPoints.Length == 0) return;

        int attempts = 0;
        const int MaxAttempts = 4; // ��ֹ��ѭ��
        do
        {
            enemy.CurrentPatrolIndex = (enemy.CurrentPatrolIndex + 1) % enemy.PatrolPoints.Length;
            _targetPoint = enemy.PatrolPoints[enemy.CurrentPatrolIndex].position;
            attempts++;
        } while (PathBlocked() && attempts < MaxAttempts);
    }

    /// <summary>
    /// ���ǰ��һ���Ƕ��Լ��������Ƿ����ϰ���
    /// </summary>
    private bool PathBlocked()
    {
        Vector3 dir = (_targetPoint - enemy.transform.position).normalized;
        Vector3 origin = enemy.transform.position + Vector3.up * 0.3f;
        return Physics.SphereCast(origin, CheckRadius, dir, out _, CheckDistance);
    }
}