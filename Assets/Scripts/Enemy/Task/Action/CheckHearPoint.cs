using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace KidGame.Core
{
    /// <summary>
    /// �������������λ�õ�
    /// </summary>
    public class CheckHearPoint : BaseEnemyAction
    {
        [Header("���������")]
        [SerializeField] private float arriveThreshold = 0.5f; // �����ж���ֵ
        private bool isMoving = false;
        private NavMeshAgent navAgent;
        private TaskStatus currentStatus = TaskStatus.Running; // ״̬��־

        public override void OnStart()
        {
            base.OnStart();
            currentStatus = TaskStatus.Running; // ����״̬
            
            // ��ȡ�������
            navAgent = enemy.GetComponent<NavMeshAgent>();
            if (navAgent == null)
            {
                Debug.LogError("����ȱ��NavMeshAgent�����");
                currentStatus = TaskStatus.Failure;
                return;
            }
            
            // ����Ѱ·
            enemy.GoCheckHearPoint();
            isMoving = true;
            
            // ����������Э��
            StartCoroutine(CheckArriveCoroutine());
        }

        public override TaskStatus OnUpdate()
        {
            // ֱ�ӷ��ص�ǰ״̬����Э�̸��£�
            return currentStatus;
        }

        // ����Э�̣���⵽��Ŀ���
        private IEnumerator CheckArriveCoroutine()
        {
            yield return null; // �ȴ�һ֡ȷ��·����ʼ����

            while (isMoving && navAgent.enabled)
            {
                // 1. ·�������У��������
                if (navAgent.pathPending)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                // 2. ���·����Ч��
                if (navAgent.pathStatus != NavMeshPathStatus.PathComplete)
                {
                    Debug.LogWarning("����·����Ч�򲻿ɴ");
                    currentStatus = TaskStatus.Failure; // ����ʧ��״̬
                    isMoving = false;
                    yield break;
                }

                // 3. ����Ƿ񵽴�Ŀ��
                if (navAgent.remainingDistance <= arriveThreshold)
                {
                    currentStatus = TaskStatus.Success; // ���óɹ�״̬
                    isMoving = false;
                    yield break;
                }

                yield return new WaitForEndOfFrame(); // ÿ֡���һ��
            }

            // 4. �쳣�˳�ʱ����ʧ��
            if (isMoving)
            {
                currentStatus = TaskStatus.Failure;
                isMoving = false;
            }
        }

        // �����ж�ʱ����
        public override void OnEnd()
        {
            isMoving = false;
            currentStatus = TaskStatus.Running; // ����״̬
            StopAllCoroutines();
        }
    }
}
