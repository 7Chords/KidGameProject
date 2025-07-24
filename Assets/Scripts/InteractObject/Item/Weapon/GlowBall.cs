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


        #region 贝塞尔曲线相关变量
        private Vector3[] bezierPoints; // 贝塞尔曲线点集合
        private int currentPointIndex = 0;
        private bool isFollowingBezier = false;
        private float bezierSpeed = 5f; // 贝塞尔曲线移动速度
        private bool isSimulateEnd = false;
        #endregion
        protected override void Awake()
        {
            // 初始化贝塞尔曲线
            if (lineRenderScript != null && lineRenderScript.points != null)
            {
                bezierPoints = lineRenderScript.points;
                isFollowingBezier = bezierPoints.Length > 1;

                if (isFollowingBezier)
                {
                    transform.position = bezierPoints[0]; // 设置初始位置为第一个点
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
            // 如果在手上就持续更新贝塞尔曲线的点集
            // 否则开始移动
            if(isOnHand) SetbezierPoints(bezierPoints);
            else
            {
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
            if (isSimulateEnd) return;

            // 如果到达目的地了 
            if(Vector3.Distance(transform.position, lineRenderScript.endPoint) <= 0.05f)
            {
                // 半径内的所有碰撞体 是否有敌人
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
                // 如果附近没目标 直接消失 也许应该延时？
                // 否则吸附上去 持续 X秒 协程计时
                if (!isEnemyHere || enemyCollider == null) Destroy(gameObject);
                else
                {
                    if(!isStartCoroutine) StartCoroutine(AttachAndDestroySelf());
                }

                return;
            }
            // 朝着目标移动
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
}