using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class GlowBall : WeaponBase
    {
        private float checkRadius = 3f;
        private bool isEnemyHere = false;
        private bool isStartCoroutine = false;
        private Collider enemyCollider = null;


        #region ������������ر���
        private Vector3[] bezierPoints; // ���������ߵ㼯��
        private int currentPointIndex = 0;
        private bool isFollowingBezier = false;
        private float bezierSpeed = 5f; // �����������ƶ��ٶ�
        #endregion
        protected override void Awake()
        {
            // ��ʼ������������
            if (lineRenderScript != null && lineRenderScript.points != null)
            {
                bezierPoints = lineRenderScript.points;
                isFollowingBezier = bezierPoints.Length > 1;

                if (isFollowingBezier)
                {
                    transform.position = bezierPoints[0]; // ���ó�ʼλ��Ϊ��һ����
                }
            }
        }
        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            if (isFollowingBezier && bezierPoints != null && currentPointIndex < bezierPoints.Length)
            {
                FollowBezierPath();
            }
        }

        public override void WeaponUseLogic()
        {
            // �����û�����յ� ��Ҫִ����δ���
            if (isFollowingBezier) return;

            // �������Ŀ�ĵ��� 
            if(Vector3.Distance(transform.position, lineRenderScript.endPoint) <= 0.05f)
            {
                // �뾶�ڵ�������ײ�� �Ƿ��е���
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius);

                foreach (Collider collider in hitColliders)
                {
                    if (isEnemyHere == true) break;
                    if (collider.CompareTag("Enemy") && collider)
                    {
                        enemyCollider = collider;
                        isEnemyHere = true;
                        break;
                    }
                }
                // �������ûĿ�� ֱ����ʧ Ҳ��Ӧ����ʱ��
                // ����������ȥ ���� X�� Э�̼�ʱ
                if (!isEnemyHere || enemyCollider == null) Destroy(gameObject);
                else
                {
                    if(!isStartCoroutine) StartCoroutine(AttachAndDestroySelf());
                }

                return;
            }
            // ����Ŀ���ƶ�
        }

        IEnumerator AttachAndDestroySelf()
        {
            // �ڶ��������� �Ӹ���������λ������Ϊ0 ֱ�Ӵ�ģ
            if(enemyCollider != null) this.gameObject.transform.SetParent(enemyCollider.transform, false);
            isStartCoroutine = true;
            yield return new WaitForSeconds(10f); // �ס��ʱ��
            Destroy(gameObject);
        }
        private void FollowBezierPath()
        {
            Vector3 targetPoint = bezierPoints[currentPointIndex];

            // ������벢�ж��Ƿ��ѵ���
            float distance = Vector3.Distance(transform.position, targetPoint);
            if (distance <= 0.05f)
            {
                // ֱ�����õ�Ŀ��㣬���⸡����������
                transform.position = targetPoint;
                currentPointIndex++;
                // ��� �Ѿ����������С ��ô��һ�β���ģ�� ת��������Ŀ�����߼�
                if (currentPointIndex >= bezierPoints.Length)
                {
                    isFollowingBezier = false;
                }
            }
            else
            {
                // �����ƶ�
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPoint,
                    bezierSpeed * Time.deltaTime
                );
            }
        }

    }
}