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
    public class ThrowWeaponBase : WeaponBase
    {
        [SerializeField] protected Rigidbody rb; // �������������Inspector��ֵ��
        protected float checkRadius = 3f;         // �����˵İ뾶
        protected bool isEnemyHere = false;
        protected Collider enemyCollider = null;
        protected bool isSimulateEnd = false;     // �Ƿ񵽴��յ�����
        protected LineRenderer lineRenderer;
        protected LineRenderScript lineRenderScript;
        protected bool isCheckCondition = false;
        // �������������Inspector������
        [SerializeField] protected float baseHorizontalSpeed = 10f; // ˮƽ��׼�ٶ�

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
            InitLineRender();
            // �ֳ�ʱǿ�ƽ�������
            if (isNotUse && rb != null)
            {
                rb.isKinematic = true;
            }
        }

        protected override void Update()
        {
            if (!isNotUse) WeaponUseLogic();
            else
            {
                SetStartPoint();
                SetEndPoint();
            }
            // �ֳ�ʱ�����������
            if (isNotUse && rb != null && !rb.isKinematic)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            // �Ƿ�Ӧ��ֹͣ�ж�
            CheckIfReachCondition();
        }

        public override void SetIsNotUse(bool onHand)
        {
            base.SetIsNotUse(onHand);
            // ��������ع켣Ԥ��
            if (!onHand && lineRenderer != null)
                lineRenderer.enabled = false;
        }
        // ����Ƿ���ĳ������
        protected void CheckIfReachCondition()
        {
            _CheckIfReachCondition();
        }

        protected virtual void _CheckIfReachCondition()
        {

        }

        // ���㲢Ӧ��������ٶȣ������߼���
        protected void CalculateAndApplyInitialVelocity()
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

        protected void InitLineRender()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderScript = GetComponent<LineRenderScript>();
            if (lineRenderScript != null && lineRenderer != null)
                lineRenderScript.lineRenderer = lineRenderer;

            // ��ʼ�������յ�
            if (PlayerController.Instance != null)
                lineRenderScript.startPoint = PlayerController.Instance.PlaceTrapPoint.position;
            lineRenderScript.endPoint = MouseRaycaster.Instance.GetMousePosi();
        }

        protected void SetEndPoint()
        {
            if (lineRenderScript != null)
            {
                Vector3 mousePos = MouseRaycaster.Instance.GetMousePosi();
                if (mousePos != Vector3.zero)
                    lineRenderScript.endPoint = mousePos;
            }
        }

        // ������������㣨����λ�ã�
        protected void SetStartPoint()
        {
            if (lineRenderScript != null)
            {
                if (PlayerController.Instance != null)
                    lineRenderScript.startPoint = PlayerController.Instance.PlaceTrapPoint.position;
                else
                    lineRenderScript.startPoint = transform.position;
            }
        }

        protected virtual void OnDestroy()
        {
        }
        public override void _WeaponUseLogic() { }
    }
}