using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class CanSeePlayer : BaseEnemyConditional
    {
        public override TaskStatus OnUpdate()
        {
            if(enemy.PlayerInSight())
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
