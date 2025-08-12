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
    public class GlowBall : WeaponBase
    {
        [SerializeField] private Rigidbody rb; // �������������Inspector��ֵ��
        private float checkRadius = 3f;         // �����˵İ뾶
        private bool isEnemyHere = false;
        private bool isStartCoroutine = false;
        private Collider enemyCollider = null;
        private bool isSimulateEnd = false;     // �Ƿ񵽴��յ�����

        // �������������Inspector������
        [SerializeField] private float baseHorizontalSpeed = 10f; // ˮƽ��׼�ٶ�

        protected override void Awake()
        {
            base.Awake();
            // ��ʼ���������
            if (rb == null)
                rb = GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;    // ��ʼ���������ֳ�״̬��
                rb.useGravity = true;     // ���ã�����ʱ����
                rb.drag = 0;              // ������
                rb.angularDrag = 0;       // ����ת����
            }
        }

        protected override void Start()
        {
            base.Start();
            // �ֳ�ʱǿ�ƽ�������
            if (isOnHand && rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        protected override void Update()
        {
            base.Update();
            // �ֳ�ʱ�����������
            if (isOnHand && rb != null && !rb.isKinematic)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            // ��������Ƿ񵽴��յ�
            else if (!isOnHand && !isSimulateEnd)
            {
                CheckIfReachEnd();
            }
        }

        // ����Ƿ񵽴��յ�����
        private void CheckIfReachEnd()
        {
            if (lineRenderScript == null) return;

            float distanceToEnd = Vector3.Distance(transform.position, lineRenderScript.endPoint);
            if (distanceToEnd <= 0.5f) // �յ�����뾶���ɵ�����
            {
                isSimulateEnd = true;
            }
        }

        public override void WeaponUseLogic()
        {
            if (isOnHand) return; // ���ڷ����ִ��

            // �շ���ʱ����������Ӧ�ó��ٶ�
            if (!isSimulateEnd && rb != null && rb.isKinematic)
            {
                rb.isKinematic = false; // ��������
                CalculateAndApplyInitialVelocity();
            }
            // �����յ��ִ�е��˼���߼�
            else if (isSimulateEnd)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius);
                isEnemyHere = false;
                enemyCollider = null;

                foreach (Collider collider in hitColliders)
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        isEnemyHere = true;
                        enemyCollider = collider;
                        break;
                    }
                }

                // �޵��������٣��е���������
                if (!isEnemyHere || enemyCollider == null)
                {
                    Destroy(gameObject, 0.5f); // �ӳ����٣�������
                }
                else if (!isStartCoroutine)
                {
                    StartCoroutine(AttachAndDestroySelf());
                }
            }
        }

        // ���㲢Ӧ��������ٶȣ������߼���
        private void CalculateAndApplyInitialVelocity()
        {
            if (lineRenderScript == null || rb == null) return;

            Vector3 start = lineRenderScript.startPoint;
            Vector3 end = lineRenderScript.endPoint;

            // 1. ����ˮƽ����XZƽ�棩
            Vector3 horizontalStart = new Vector3(start.x, 0, start.z);
            Vector3 horizontalEnd = new Vector3(end.x, 0, end.z);
            Vector3 horizontalDir = (horizontalEnd - horizontalStart).normalized;
            float horizontalDistance = Vector3.Distance(horizontalStart, horizontalEnd);

            // 2. �����˶���ʱ�䣨����ˮƽ����ͻ�׼�ٶȣ�
            float T = horizontalDistance / baseHorizontalSpeed;
            T = Mathf.Max(T, 0.1f); // ����ʱ����̵����ٶ��쳣

            // 3. ���㴹ֱ������ٶȣ���������ʽ��
            float verticalDisplacement = end.y - start.y;
            float gravity = Physics.gravity.y; // �������ٶȣ�UnityĬ��-9.81��
            float verticalVelocity = (verticalDisplacement - 0.5f * gravity * T * T) / T;

            // 4. Ӧ�����ճ��ٶ�
            Vector3 initialVelocity = horizontalDir * baseHorizontalSpeed;
            initialVelocity.y = verticalVelocity;
            rb.velocity = initialVelocity;
        }

        // ���������˲��ӳ�����
        IEnumerator AttachAndDestroySelf()
        {
            isStartCoroutine = true;
            if (enemyCollider != null)
            {
                // ���������ˣ������λ��ƫ�ƣ�
                transform.SetParent(enemyCollider.transform, false);
                // �������������������
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
            }

            yield return new WaitForSeconds(10f); // ����10�루�ɵ�����
            Destroy(gameObject);
        }
    }
}