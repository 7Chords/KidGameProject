using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 检查听到声音的位置点
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
