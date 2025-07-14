using System.Collections;
using UnityEngine;

namespace KidGame.Core
{
    [CreateAssetMenu(fileName = "DashSkill", menuName = "KidGameSO/Enemy/DashSkill")]
    public class DashSkill : SkillBase
    {
        [Header("冲刺参数")]
        public float dashDistance = 5f;
        public float dashSpeed = 15f;
        public float triggerRange = 8f;
        public LayerMask obstacleLayer;
        
        [System.NonSerialized] public EnemyController enemy;
        private Vector3 dashDirection;
        private UnityEngine.AI.NavMeshAgent navAgent; // 缓存导航组件

        public override void Init(EnemyController enemyController)
        {
            enemy = enemyController;
            navAgent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        public override bool CanTrigger()
        {
            if (IsInCooldown() || enemy == null || enemy.Player == null)
                return false;

            float distance = Vector3.Distance(enemy.transform.position, enemy.Player.position);
            return distance <= triggerRange && distance > 1f;
        }

        // 原Trigger方法改造为返回协程
        public IEnumerator TriggerCoroutine()
        {
            Debug.Log("dashing!!");
            // 记录开始时间（用于冷却计算）
            lastCastTime = Time.time;
            
            // 计算目标位置
            dashDirection = (enemy.Player.position - enemy.transform.position).normalized;
            dashDirection.y = 0;
            Vector3 startPosition = enemy.transform.position;
            Vector3 targetPosition = startPosition + dashDirection * dashDistance;

            // 障碍物检测
            if (Physics.Raycast(startPosition, dashDirection, out RaycastHit hit, dashDistance, obstacleLayer))
            {
                targetPosition = hit.point - dashDirection * 0.5f;
            }

            // 执行冲刺
            if (navAgent != null) navAgent.enabled = false;
            //enemy.Animator.SetTrigger("Dash"); // 假设EnemyController有Animator属性

            float distanceTraveled = 0;
            float totalDistance = Vector3.Distance(startPosition, targetPosition);
            
            while (distanceTraveled < totalDistance)
            {
                float moveStep = dashSpeed * Time.deltaTime;
                enemy.transform.position = Vector3.MoveTowards(
                    enemy.transform.position, targetPosition, moveStep);
                distanceTraveled += moveStep;
                yield return null; // 每帧更新位置
            }

            // 冲刺结束恢复状态
            if (navAgent != null) navAgent.enabled = true;
        }

        // 外部终止冲刺的方法
        public void StopDash()
        {
            if (navAgent != null && !navAgent.enabled)
            {
                navAgent.enabled = true; // 恢复导航
            }
        }
    }
}