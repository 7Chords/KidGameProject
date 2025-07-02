using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ×·»÷Íæ¼Ò½Úµã
    /// </summary>
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
                PlayerController.Instance.TakeDamage(new DamageInfo(gameObject, 1));
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
}
