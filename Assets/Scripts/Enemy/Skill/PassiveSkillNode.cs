using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace KidGame.Core
{


    /// <summary>
    /// �������ܼ���Ƿ���Ч�Ľڵ�
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
            // �������б������ܣ���鴥������
            foreach (var skill in passiveSkills)
            {
                if (!skill.IsInCooldown() && skill.CanTrigger())
                {
                    skill.Trigger();
                    skill.lastCastTime = Time.time;
                }
            }

            return TaskStatus.Failure; // ����false����Ӱ��ԭ��Ϊ������
        }
        
    }
}