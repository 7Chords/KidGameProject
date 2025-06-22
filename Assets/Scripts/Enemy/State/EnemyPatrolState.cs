using KidGame.Core;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 巡逻，在路径点间行走。碰到障碍或感知玩家时做相应处理
    /// </summary>
    public class EnemyPatrolState : EnemyStateBase
    {
        private Vector3 _targetPoint;

        private const float CheckDistance = 1.5f;
        private const float CheckRadius = 0.25f;
        private const float MinTargetThreshold = 0.2f;

        private float _idleTimer;
        private const float IdleDuration = 2.0f;
        private float _searchTriggerTimer;
        private const float SearchTriggerInterval = 10.0f;

        public override void Enter()
        {
            enemy.PatrolTimer = 0f;
            PickNextPoint();
            // enemy.PlayAnimation("Walk");
        }

        public override void Update()
        {
            _idleTimer += Time.deltaTime;
            _searchTriggerTimer += Time.deltaTime;

            #region 常规巡逻

            // 侦测玩家
            if (enemy.PlayerInSight())
            {
                Debug.Log("AttackPlayer");
                enemy.ChangeState(EnemyState.Attack);
                return;
            }

            // 若无巡逻点
            if (enemy.PatrolPoints == null || enemy.PatrolPoints.Length == 0)
            {
                enemy.ChangeState(EnemyState.Idle);
                return;
            }

            // 如果路径被阻挡，换目标
            if (PathBlocked())
            {
                PickNextPoint();
                return;
            }

            // 正常移动
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

            #endregion


            // 检查是否触发有目的搜索
            if (_searchTriggerTimer >= SearchTriggerInterval)
            {
                _searchTriggerTimer = 0f;

                // 30%几率触发有目的搜索
                if (Random.value < 0.3f)
                {
                    string targetItem = GetRandomSearchItem();
                    if (!string.IsNullOrEmpty(targetItem))
                    {
                        enemy._currentTargetItemId = targetItem;
                        enemy.ChangeState(EnemyState.SearchTargetRoom);
                    }
                }
            }
        }

        public override void Exit() => enemy.Rb.velocity = Vector3.zero;

        /// <summary>
        /// 选取下一巡逻点，顺序循环。若连续阻挡将尝试多次
        /// </summary>
        private void PickNextPoint()
        {
            if (enemy.PatrolPoints == null || enemy.PatrolPoints.Length == 0) return;

            int attempts = 0;
            const int MaxAttempts = 4; // 防止死循环
            do
            {
                enemy.CurrentPatrolIndex = (enemy.CurrentPatrolIndex + 1) % enemy.PatrolPoints.Length;
                _targetPoint = enemy.PatrolPoints[enemy.CurrentPatrolIndex].position;
                attempts++;
            } while (PathBlocked() && attempts < MaxAttempts);
        }

        /// <summary>
        /// 检测前方一定角度以及距离内是否有障碍物
        /// </summary>
        private bool PathBlocked()
        {
            Vector3 dir = (_targetPoint - enemy.transform.position).normalized;
            Vector3 origin = enemy.transform.position + Vector3.up * 0.3f;
            return Physics.SphereCast(origin, CheckRadius, dir, out _, CheckDistance);
        }
        
        private string GetRandomSearchItem()
        {
            // 从游戏数据中获取一个随机物品ID
            string[] possibleItems = { "dog"};
            return possibleItems[Random.Range(0, possibleItems.Length)];
        }
    }
}