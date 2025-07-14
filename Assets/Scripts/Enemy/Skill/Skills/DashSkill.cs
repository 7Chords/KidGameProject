using System.Collections;
using UnityEngine;

namespace KidGame.Core
{
    [CreateAssetMenu(fileName = "DashSkill", menuName = "KidGameSO/Enemy/DashSkill")]
    public class DashSkill : SkillBase
    {
        [Header("��̲���")]
        public float dashDistance = 5f;
        public float dashSpeed = 15f;
        public float triggerRange = 8f;
        public LayerMask obstacleLayer;
        
        [System.NonSerialized] public EnemyController enemy;
        private Vector3 dashDirection;
        private UnityEngine.AI.NavMeshAgent navAgent; // ���浼�����

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

        // ԭTrigger��������Ϊ����Э��
        public IEnumerator TriggerCoroutine()
        {
            Debug.Log("dashing!!");
            // ��¼��ʼʱ�䣨������ȴ���㣩
            lastCastTime = Time.time;
            
            // ����Ŀ��λ��
            dashDirection = (enemy.Player.position - enemy.transform.position).normalized;
            dashDirection.y = 0;
            Vector3 startPosition = enemy.transform.position;
            Vector3 targetPosition = startPosition + dashDirection * dashDistance;

            // �ϰ�����
            if (Physics.Raycast(startPosition, dashDirection, out RaycastHit hit, dashDistance, obstacleLayer))
            {
                targetPosition = hit.point - dashDirection * 0.5f;
            }

            // ִ�г��
            if (navAgent != null) navAgent.enabled = false;
            //enemy.Animator.SetTrigger("Dash"); // ����EnemyController��Animator����

            float distanceTraveled = 0;
            float totalDistance = Vector3.Distance(startPosition, targetPosition);
            
            while (distanceTraveled < totalDistance)
            {
                float moveStep = dashSpeed * Time.deltaTime;
                enemy.transform.position = Vector3.MoveTowards(
                    enemy.transform.position, targetPosition, moveStep);
                distanceTraveled += moveStep;
                yield return null; // ÿ֡����λ��
            }

            // ��̽����ָ�״̬
            if (navAgent != null) navAgent.enabled = true;
        }

        // �ⲿ��ֹ��̵ķ���
        public void StopDash()
        {
            if (navAgent != null && !navAgent.enabled)
            {
                navAgent.enabled = true; // �ָ�����
            }
        }
    }
}