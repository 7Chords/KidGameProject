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

            // 3. 启动冲刺协程并跟踪状态
            dashCoroutine = enemy.StartCoroutine(DashCoroutineWrapper());
            return TaskStatus.Running;
        }
        // 协程包装器（用于状态管理）
        private IEnumerator DashCoroutineWrapper()
        {
            isDashing = true;
            yield return dashSkill.TriggerCoroutine(); // 执行实际冲刺逻辑
            isDashing = false; // 协程结束，重置状态
            dashCoroutine = null;
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