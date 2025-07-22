using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace KidGame.Core
{


    /// <summary>
    /// 被动技能检测是否生效的节点
    /// </summary>
    public class PassiveSkillNode : BaseEnemyAction
    {
        [SerializeField] private List<SkillBase> passiveSkills = new List<SkillBase>();
        public override void OnAwake()
        {
            base.OnAwake();
            foreach (var skill in passiveSkills)
            {
                skill.Init(enemy);
            }
        }

        public override TaskStatus OnUpdate()
        {
            // 遍历所有被动技能，检查触发条件
            foreach (var skill in passiveSkills)
            {
                if (!skill.IsInCooldown() && skill.CanTrigger())
                {
                    skill.Trigger();
                    skill.lastCastTime = Time.time;
                }
            }

            return TaskStatus.Failure; // 返回false，不影响原行为树流程
        }
        
    }
}