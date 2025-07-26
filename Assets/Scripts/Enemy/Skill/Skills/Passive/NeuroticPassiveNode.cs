// NeuroticPassiveNode.cs
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using UnityEngine;

namespace KidGame.Core
{
    [TaskCategory("Enemy/Passive")]
    public class NeuroticPassiveNode : BaseEnemyAction
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("神经质被动技能配置")]
        [SerializeField] private NeuroticPassiveSkill neuroticSkill;

        private Coroutine stopCoroutine; // 停止协程引用
        private bool isSkillInitialized;

        public override void OnAwake()
        {
            base.OnAwake();
            if (neuroticSkill != null && !isSkillInitialized)
            {
                neuroticSkill.Init(enemy);
                isSkillInitialized = true;
            }
        }

        public override TaskStatus OnUpdate()
        {
            // 技能未配置或未初始化 → 返回Failure
            if (neuroticSkill == null || !isSkillInitialized)
                return TaskStatus.Failure;

            // 正在停止中 → 返回Running阻止其他节点
            if (neuroticSkill.isStopping)
                return TaskStatus.Running;

            // 检查是否达到触发条件
            if (neuroticSkill.CheckTriggerCondition())
            {
                stopCoroutine = enemy.StartCoroutine(neuroticSkill.StartStopCoroutine());
                return TaskStatus.Running; // 首次触发返回Running
            }

            // 平常状态返回Failure，不干扰其他节点
            return TaskStatus.Failure;
        }

        // 节点被中断时清理协程（如被更高优先级技能打断）
        public override void OnEnd()
        {
            if (stopCoroutine != null)
            {
                enemy.StopCoroutine(stopCoroutine);
                stopCoroutine = null;
                neuroticSkill.ForceEndStop(); // 强制恢复移动
            }
        }

        // 重置节点状态（行为树重启时）
        public override void OnReset()
        {
            OnEnd();
            isSkillInitialized = false;
        }
    }
}