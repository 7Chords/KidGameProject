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

        [Header("���׼������")] // ��������
        public float prepareTime = 0.5f; // ׼��ʱ�䣨�룩
        public float rotateSpeed = 15f; // ת���ٶ�
        
        [System.NonSerialized] public EnemyController enemy;
        private Vector3 dashDirection;
        private UnityEngine.AI.NavMeshAgent navAgent; // ���浼�����

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

        // �޸ģ�����Ŀ��λ�ò���������ʵʱ��ȡ���λ�ã�
        public IEnumerator TriggerCoroutine(Vector3 targetPosition)
        {
            Debug.Log("Enemy dashing!");
            lastCastTime = Time.time;

            Vector3 startPosition = enemy.transform.position;
            dashDirection = (targetPosition - startPosition).normalized;
            dashDirection.y = 0;

            // �ϰ�����
            Vector3 desiredTarget = startPosition + dashDirection * dashDistance;
            if (Physics.Raycast(
                    startPosition + Vector3.up, 
                    dashDirection, 
                    out RaycastHit hit, 
                    dashDistance, 
                    obstacleLayer))
            {
                desiredTarget = hit.point - dashDirection * 0.5f; // ͣ���ϰ���ǰ
            }

            // ִ�г��
            if (navAgent != null) navAgent.enabled = false;
            // enemy.Animator.SetTrigger("Dash"); // ȡ��ע���Բ��Ŷ���

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

            // �ָ�����
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