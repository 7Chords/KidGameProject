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


        #region 贝塞尔曲线相关变量
        private Vector3[] bezierPoints; // 贝塞尔曲线点集合
        private int currentPointIndex = 0;
        private bool isFollowingBezier = true;
        private float bezierSpeed = 10f; // 贝塞尔曲线移动速度
        private bool isSimulateEnd = false;
        #endregion
        protected override void Awake()
        {
            // 初始化贝塞尔曲线
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
                    transform.position = bezierPoints[0]; // 设置初始位置为第1个点
                }
                for (int i = 1; i < bezierPoints.Length; i++)
                    Debug.Log(bezierPoints[i]);
            }
        }
        protected override void Update()
        {
            base.Update();
            // 如果在手上就持续更新贝塞尔曲线的点集
            // 否则开始移动
            if(isOnHand) SetbezierPoints(bezierPoints);
            else
            {
                // 进入了这里
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
            // 如果还没到达终点 不要执行这段代码
            if (!isSimulateEnd) return;
            // 如果到达目的地了 
            if (Vector3.Distance(transform.position, lineRenderScript.endPoint) <= 0.05f)
            {
                // 半径内的所有碰撞体 是否有敌人
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
                // 如果附近没目标 直接消失 也许应该延时？
                // 否则吸附上去 持续 X秒 协程计时
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
            // 第二个参数把 子父物体的相对位置设置为0 直接穿模
            if(enemyCollider != null) this.gameObject.transform.SetParent(enemyCollider.transform, false);
            isStartCoroutine = true;
            yield return new WaitForSeconds(10f); // 黏住的时间
            Destroy(gameObject);
        }
        private void FollowBezierPath()
        {
            Vector3 targetPoint = bezierPoints[currentPointIndex];

            // 计算距离并判断是否已到达
            float distance = Vector3.Distance(transform.position, targetPoint);
            if (distance <= 0.05f)
            {
                // 直接设置到目标点，避免浮点数误差积累
                transform.position = targetPoint;
                currentPointIndex++;
                // 如果 已经超出数组大小 那么下一次不再模拟 转而处理到达目标点的逻辑
                if (currentPointIndex >= bezierPoints.Length)
                {
                    isFollowingBezier = false;
                    isSimulateEnd = true;
                }
            }
            else
            {
                // 正常移动
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
        [SerializeField] protected Rigidbody rb; // 物理组件（需在Inspector赋值）
        protected float checkRadius = 3f;         // 检测敌人的半径
        protected bool isEnemyHere = false;
        protected Collider enemyCollider = null;
        protected bool isSimulateEnd = false;     // 是否到达终点区域
        protected LineRenderer lineRenderer;
        protected LineRenderScript lineRenderScript;
        protected bool isCheckCondition = false;
        // 物理参数（可在Inspector调整）
        [SerializeField] protected float baseHorizontalSpeed = 10f; // 水平基准速度

        protected override void Awake()
        {
            base.Awake();
            // 初始化物理组件
            if (rb == null)
                rb = GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;    // 初始禁用物理（手持状态）
                rb.useGravity = true;     // 备用：发射时启用
                rb.drag = 0;              // 无阻力
                rb.angularDrag = 0;       // 无旋转阻力
            }
        }

        protected override void Start()
        {
            base.Start();
            InitLineRender();
            // 手持时强制禁用物理
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
            // 手持时保持物理禁用
            if (isNotUse && rb != null && !rb.isKinematic)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            // 是否应当停止判断
            CheckIfReachCondition();
        }

        public override void SetIsNotUse(bool onHand)
        {
            base.SetIsNotUse(onHand);
            // 发射后隐藏轨迹预览
            if (!onHand && lineRenderer != null)
                lineRenderer.enabled = false;
        }
        // 检测是否达成某个条件
        protected void CheckIfReachCondition()
        {
            _CheckIfReachCondition();
        }

        protected virtual void _CheckIfReachCondition()
        {

        }

        // 计算并应用物理初速度（核心逻辑）
        protected void CalculateAndApplyInitialVelocity()
        {
            if (lineRenderScript == null || rb == null) return;

            Vector3 start = lineRenderScript.startPoint;
            Vector3 end = lineRenderScript.endPoint;

            // 1. 计算水平方向（XZ平面）
            Vector3 horizontalStart = new Vector3(start.x, 0, start.z);
            Vector3 horizontalEnd = new Vector3(end.x, 0, end.z);
            Vector3 horizontalDir = (horizontalEnd - horizontalStart).normalized;
            float horizontalDistance = Vector3.Distance(horizontalStart, horizontalEnd);

            // 2. 计算运动总时间（基于水平距离和基准速度）
            float T = horizontalDistance / baseHorizontalSpeed;
            T = Mathf.Max(T, 0.1f); // 避免时间过短导致速度异常

            // 3. 计算垂直方向初速度（基于物理公式）
            float verticalDisplacement = end.y - start.y;
            float gravity = Physics.gravity.y; // 重力加速度（Unity默认-9.81）
            float verticalVelocity = (verticalDisplacement - 0.5f * gravity * T * T) / T;

            // 4. 应用最终初速度
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

            // 初始化起点和终点
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

        // 设置抛物线起点（武器位置）
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