using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// �������������λ�õ�
    /// </summary>
    public class CheckHearPoint : BaseEnemyAction
    {
        public override void OnStart()
        {
            enemy.GoCheckHearPoint();
        }

        public override TaskStatus OnUpdate()
        {
            if(enemy.CheckArriveDestination(true))
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
}
