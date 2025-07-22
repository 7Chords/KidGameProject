using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class DashSkillNode : BaseEnemyAction {
        [SerializeField] private DashSkill dashSkill; // ���������ʲ�
        private bool isDashing = false; // Э��״̬���
        private Coroutine dashCoroutine; // Э������

        public override void OnAwake()
        {
            base.OnAwake();
            dashSkill.Init(enemy);
        }


        public override TaskStatus OnUpdate() {
            // 1. Э��ִ���� �� ����Running�����ڵ�
            if (isDashing)
            {
                return TaskStatus.Running;
            }

            // 2. ��鼼���Ƿ�ɴ���
            if (!dashSkill.CanTrigger())
            {
                return TaskStatus.Failure;
            }

            // ����׼�����̵�����Э�̣������޸ģ�
            dashCoroutine = enemy.StartCoroutine(PrepareAndDashCoroutine());
            return TaskStatus.Running;
        }
        
        // ������׼����̵���������Э��
        private IEnumerator PrepareAndDashCoroutine()
        {
            isDashing = true;

            // 1. ��¼��ҵ�ǰλ�ã��������Ŀ�꣩
            Vector3 recordedPlayerPos = enemy.Player.position;
            enemy.Rb.velocity = Vector3.zero;
            dashSkill.StopNavAgent();
            // 2. ƽ��ת���¼�����λ��
            yield return StartCoroutine(TurnToTarget(recordedPlayerPos));

            // 3. �ȴ����׼��ʱ�䣨�����ã�
            yield return new WaitForSeconds(dashSkill.prepareTime);

            // 4. ִ�г�̣�ʹ��������λ�ã�
            yield return enemy.StartCoroutine(dashSkill.TriggerCoroutine(recordedPlayerPos));

            // 5. ����״̬
            isDashing = false;
            dashCoroutine = null;
        }
        // ������ƽ��ת��Э��
        private IEnumerator TurnToTarget(Vector3 targetPosition)
        {
            Vector3 lookDirection = (targetPosition - enemy.transform.position).normalized;
            lookDirection.y = 0; // ����Y�ᣬ����ƽ����ת

            if (lookDirection.sqrMagnitude < 0.01f) yield break; // Ŀ���������ת��

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            float rotateSpeed = dashSkill.rotateSpeed;

            // ƽ����ֵ��ת
            while (Quaternion.Angle(enemy.transform.rotation, targetRotation) > 10f)
            {
                enemy.transform.rotation = Quaternion.Lerp(
                    enemy.transform.rotation,
                    targetRotation,
                    rotateSpeed * Time.deltaTime
                );
                yield return null;
            }
            enemy.transform.rotation = targetRotation; // ȷ��������ת��λ
        }
        

        // �ڵ㱻�ж�ʱ��ֹЭ�̣��类�������ȼ���Ϊ��ϣ�
        public override void OnEnd()
        {
            if (dashCoroutine != null)
            {
                enemy.StopCoroutine(dashCoroutine);
                isDashing = false;
                dashCoroutine = null;
                dashSkill.StopDash(); // ֪ͨ������ֹ���
            }
        }
    }
}