/*using System.Collections;
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
        private bool isFollowingBezier = true;
        private float bezierSpeed = 10f; // �����������ƶ��ٶ�
        private bool isSimulateEnd = false;
        #endregion
        protected override void Awake()
        {
            // ��ʼ������������
            this.InitLineRender();
        }

        protected override void Start()
        {
            base.Start();
            if (lineRenderScript != null && lineRenderScript.GetPoints() != null && !isOnHand)
            {
                bezierPoints = lineRenderScript.GetPoints();
                isFollowingBezier = bezierPoints.Length > 1;
                if (isFollowingBezier)
                {
                    transform.position = bezierPoints[0]; // ���ó�ʼλ��Ϊ��1����
                }
                for (int i = 1; i < bezierPoints.Length; i++)
                    Debug.Log(bezierPoints[i]);
            }
        }
        protected override void Update()
        {
            base.Update();
            // ��������Ͼͳ������±��������ߵĵ㼯
            // ����ʼ�ƶ�
            if(isOnHand) SetbezierPoints(bezierPoints);
            else
            {
                // ����������
                if (isFollowingBezier && bezierPoints != null && currentPointIndex < bezierPoints.Length)
                {
                    FollowBezierPath();
                }
            } 
        }

        private void SetbezierPoints(Vector3[] bezierPoints)
        {
            this.bezierPoints = bezierPoints;
        }
        public override void WeaponUseLogic()
        {
            // �����û�����յ� ��Ҫִ����δ���
            if (!isSimulateEnd) return;
            // �������Ŀ�ĵ��� 
            if (Vector3.Distance(transform.position, lineRenderScript.endPoint) <= 0.05f)
            {
                // �뾶�ڵ�������ײ�� �Ƿ��е���
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius);

                foreach (Collider collider in hitColliders)
                {
                    if (isEnemyHere == true) break;
                    if (collider.CompareTag("Enemy") && collider != null)
                    {
                        enemyCollider = collider;
                        isEnemyHere = true;
                        break;
                    }
                }
                // �������ûĿ�� ֱ����ʧ Ҳ��Ӧ����ʱ��
                // ����������ȥ ���� X�� Э�̼�ʱ
                if (!isEnemyHere || enemyCollider == null)
                {
                    Destroy(gameObject);
                }
                else
                {
                    if (!isStartCoroutine) StartCoroutine(AttachAndDestroySelf());
                }
                return;
            }
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
                    isSimulateEnd = true;
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
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class GlowBall : ThrowWeaponBase
    {

        private Vector3 hitPosi = Vector3.zero;
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }
        public override void _WeaponUseLogic()
        {
            if (isOnHand) return; // ���ڷ����ִ��
            // �շ���ʱ����������Ӧ�ó��ٶ�
            if (!isSimulateEnd && rb != null && rb.isKinematic)
            {
                rb.isKinematic = false; // ��������
                CalculateAndApplyInitialVelocity();
            }
            if(!isCheckCondition && isSimulateEnd)
            {   //ִֻ��һ�ν���ģ�⺯��
                EndSimulateAction(Physics.OverlapSphere(transform.position, checkRadius));
            }
        }

        protected void EndSimulateAction(Collider[] hitColliders)
        {
            Collider[] temphitColliders = hitColliders;
            isCheckCondition = true;// ������ֹ������
            isEnemyHere = false; // ��ʼ��
            enemyCollider = null; // ��ʼ��
            foreach (Collider collider in temphitColliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    isEnemyHere = true;
                    enemyCollider = collider;
                    hitPosi = GetHitPosition(collider);
                    break;
                }
            }
            // �޵��������٣��е���������
            if (!isEnemyHere || enemyCollider == null)
            {
                Destroy(gameObject, 0f); // �ӳ����٣�������
            }
            else StartCoroutine(AttachAndDestroySelf());

        }

        // ���ֱ��ײ����

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                isSimulateEnd = true; // ������Ҫִ�����߼�
                hitPosi = GetHitPosition(other);
                EndSimulateAction(Physics.OverlapSphere(transform.position, checkRadius));
            }
        }

        protected override void _CheckIfReachCondition()
        {
            if (lineRenderScript == null) return;
            if (rb.velocity.magnitude <= 0.05f)
            {
                // ������Ҫִ�����߼�
                isSimulateEnd = true;
            }
        }
        // ���������˲��ӳ�����
        IEnumerator AttachAndDestroySelf()
        {

            if (enemyCollider != null)
            {
                // ���������ˣ������λ��ƫ�ƣ�
                transform.position = hitPosi;
                transform.SetParent(enemyCollider.transform, true);
                // �������������������
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
            }
            yield return new WaitForSeconds(10f); // ����10�루�ɵ�����
            Destroy(gameObject);
        }

        // ��ȡ��ײ��
        private Vector3 GetHitPosition(Collider other)
        {
            Ray ray = new Ray(transform.position, (other.transform.position - transform.position).normalized);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.6f))
            {
                return hitInfo.point;
            }
            else
            {
                // ����δ����ʱ ���ضԷ���ײ�������ĵ�
                return Vector3.zero;
            }
        }
    }
}