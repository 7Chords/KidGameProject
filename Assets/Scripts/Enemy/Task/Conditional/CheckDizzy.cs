using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class CheckDizzy : BaseEnemyConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (enemy.CheckDizzyState()) return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}
