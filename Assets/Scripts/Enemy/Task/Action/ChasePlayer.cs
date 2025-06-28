using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class ChasePlayer : BaseEnemyAction
    {
        public override TaskStatus OnUpdate()
        {
            enemy.ChasePlayer();
            if(!enemy.PlayerInSight())
            {
                return TaskStatus.Failure;
            }
            if (enemy.CheckCatchPlayer())
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
}
