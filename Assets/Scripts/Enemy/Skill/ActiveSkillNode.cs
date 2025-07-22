using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace KidGame.Core
{


    /// <summary>
    /// ��������ִ�нڵ�
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
            // ���������������ܣ�����Ƿ���ͷ�
            foreach (var skill in activeSkills)
            {
                if (!skill.IsInCooldown() && skill.CanTrigger())
                {
                    // �Ӻڰ��ȡĿ��
                   
                    skill.lastCastTime = Time.time; // ������ȴʱ��
                    return TaskStatus.Success; // �ɹ��ͷ�һ�����ܺ󷵻�
                }
            }

            return TaskStatus.Failure; // �޼��ܿ��ͷ�
        }
    }
}
