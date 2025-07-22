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

        [Header("冲刺准备参数")] // 新增参数
        public float prepareTime = 0.5f; // 准备时间（秒）
        public float rotateSpeed = 15f; // 转向速度
        
        [System.NonSerialized] public EnemyController enemy;
        private Vector3 dashDirection;
        private UnityEngine.AI.NavMeshAgent navAgent; // 缓存导航组件

        public override void Init(EnemyController enemyController)
        {
            enemy = enemyController;
            navAgent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        public void StopNavAgent()
        {
            if (navAgent != null) navAgent.enabled = false;
        }

        public override bool CanTrigger()
        {
            if (IsInCooldown() || enemy == null || enemy.Player == null)
                return false;

            float distance = Vector3.Distance(enemy.transform.position, enemy.Player.position);
            return distance <= triggerRange && distance > 1f;
        }

        // 修改：接受目标位置参数（不再实时获取玩家位置）
        public IEnumerator TriggerCoroutine(Vector3 targetPosition)
        {
            Debug.Log("Enemy dashing!");
            lastCastTime = Time.time;

            Vector3 startPosition = enemy.transform.position;
            dashDirection = (targetPosition - startPosition).normalized;
            dashDirection.y = 0;

            // 障碍物检测
            Vector3 desiredTarget = startPosition + dashDirection * dashDistance;
            if (Physics.Raycast(
                    startPosition + Vector3.up, 
                    dashDirection, 
                    out RaycastHit hit, 
                    dashDistance, 
                    obstacleLayer))
            {
                desiredTarget = hit.point - dashDirection * 0.5f; // 停在障碍物前
            }

            // 执行冲刺
            if (navAgent != null) navAgent.enabled = false;
            // enemy.Animator.SetTrigger("Dash"); // 取消注释以播放动画

            float distanceTraveled = 0;
            float totalDistance = Vector3.Distance(startPosition, desiredTarget);
            while (distanceTraveled < totalDistance)
            {
                float moveStep = dashSpeed * Time.deltaTime;
                enemy.transform.position = Vector3.MoveTowards(
                    enemy.transform.position, desiredTarget, moveStep);
                distanceTraveled += moveStep;
                yield return null;
            }

            // 恢复导航
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