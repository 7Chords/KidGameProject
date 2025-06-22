using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class EnemyChaseState : EnemyStateBase
    {
        private const float DashDistance = 6f;
        private const float DashCooldown = 3f;
        private float _dashTimer;

        public override void Enter() => _dashTimer = 0f;

        public override void Update()
        {
            if (enemy.Player == null) { enemy.ChangeState(EnemyState.Idle); return; }

            _dashTimer += Time.deltaTime;
            Vector3 dir = (enemy.Player.position - enemy.transform.position).normalized;
            float  dist = Vector3.Distance(enemy.transform.position, enemy.Player.position);

            // 根据距离和随机数触发冲刺
            if ((_dashTimer >= DashCooldown && dist > DashDistance) || Random.value < 0.05f)
            {
                _dashTimer = 0f;
                enemy.StartCoroutine(Dash(dir));
            }
            else
            {
                enemy.Rb.velocity = dir * enemy.EnemyBaseData.MoveSpeed;
            }
            
            // 失去目标
            if (!enemy.PlayerInHearing())
                enemy.ChangeState(EnemyState.Idle);
            else
                enemy.ChangeState(EnemyState.Attack);
        }

        private IEnumerator Dash(Vector3 dir)
        {
            float dashTime = 0.3f;
            float dashSpeed = enemy.EnemyBaseData.MoveSpeed * 3f;
            float t = 0f;
            while (t < dashTime)
            {
                enemy.Rb.velocity = dir * dashSpeed;
                t += Time.deltaTime;
                yield return null;
            }
        }

        public override void Exit() => enemy.Rb.velocity = Vector3.zero;
    }
}
