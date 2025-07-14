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

            // 3. �������Э�̲�����״̬
            dashCoroutine = enemy.StartCoroutine(DashCoroutineWrapper());
            return TaskStatus.Running;
        }
        // Э�̰�װ��������״̬����
        private IEnumerator DashCoroutineWrapper()
        {
            isDashing = true;
            yield return dashSkill.TriggerCoroutine(); // ִ��ʵ�ʳ���߼�
            isDashing = false; // Э�̽���������״̬
            dashCoroutine = null;
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