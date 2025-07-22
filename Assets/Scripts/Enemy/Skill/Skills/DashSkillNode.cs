using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class DashSkillNode : BaseEnemyAction {
        [SerializeField] private DashSkill dashSkill; // 单个技能资产
        private bool isDashing = false; // 协程状态标记
        private Coroutine dashCoroutine; // 协程引用

        public override void OnAwake()
        {
            base.OnAwake();
            dashSkill.Init(enemy);
        }


        public override TaskStatus OnUpdate() {
            // 1. 协程执行中 → 返回Running锁定节点
            if (isDashing)
            {
                return TaskStatus.Running;
            }

            // 2. 检查技能是否可触发
            if (!dashSkill.CanTrigger())
            {
                return TaskStatus.Failure;
            }

            // 启动准备与冲刺的完整协程（核心修改）
            dashCoroutine = enemy.StartCoroutine(PrepareAndDashCoroutine());
            return TaskStatus.Running;
        }
        
        // 新增：准备冲刺的完整流程协程
        private IEnumerator PrepareAndDashCoroutine()
        {
            isDashing = true;

            // 1. 记录玩家当前位置（锁定冲刺目标）
            Vector3 recordedPlayerPos = enemy.Player.position;
            enemy.Rb.velocity = Vector3.zero;
            dashSkill.StopNavAgent();
            // 2. 平滑转向记录的玩家位置
            yield return StartCoroutine(TurnToTarget(recordedPlayerPos));

            // 3. 等待冲刺准备时间（可配置）
            yield return new WaitForSeconds(dashSkill.prepareTime);

            // 4. 执行冲刺（使用锁定的位置）
            yield return enemy.StartCoroutine(dashSkill.TriggerCoroutine(recordedPlayerPos));

            // 5. 重置状态
            isDashing = false;
            dashCoroutine = null;
        }
        // 新增：平滑转向协程
        private IEnumerator TurnToTarget(Vector3 targetPosition)
        {
            Vector3 lookDirection = (targetPosition - enemy.transform.position).normalized;
            lookDirection.y = 0; // 忽略Y轴，保持平面旋转

            if (lookDirection.sqrMagnitude < 0.01f) yield break; // 目标过近无需转向

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            float rotateSpeed = dashSkill.rotateSpeed;

            // 平滑插值旋转
            while (Quaternion.Angle(enemy.transform.rotation, targetRotation) > 10f)
            {
                enemy.transform.rotation = Quaternion.Lerp(
                    enemy.transform.rotation,
                    targetRotation,
                    rotateSpeed * Time.deltaTime
                );
                yield return null;
            }
            enemy.transform.rotation = targetRotation; // 确保最终旋转到位
        }
        

        // 节点被中断时终止协程（如被更高优先级行为打断）
        public override void OnEnd()
        {
            if (dashCoroutine != null)
            {
                enemy.StopCoroutine(dashCoroutine);
                isDashing = false;
                dashCoroutine = null;
                dashSkill.StopDash(); // 通知技能终止冲刺
            }
        }
    }
}