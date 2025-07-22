using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace KidGame.Core
{


    /// <summary>
    /// 主动技能执行节点
    /// </summary>
    public class ActiveSkillNode : BaseEnemyAction
    {
        [SerializeField] private List<SkillBase> activeSkills = new List<SkillBase>();


        public override void OnStart()
        {
            foreach (var skill in activeSkills)
            {
                skill.Init(enemy);
            }
        }

        public override TaskStatus OnUpdate()
        {
            // 遍历所有主动技能，检查是否可释放
            foreach (var skill in activeSkills)
            {
                if (!skill.IsInCooldown() && skill.CanTrigger())
                {
                    // 从黑板获取目标
                   
                    skill.lastCastTime = Time.time; // 更新冷却时间
                    return TaskStatus.Success; // 成功释放一个技能后返回
                }
            }

            return TaskStatus.Failure; // 无技能可释放
        }
    }
}
