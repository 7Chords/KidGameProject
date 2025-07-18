using BehaviorDesigner.Runtime.Tasks;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class CheckDizzy : BaseEnemyConditional
    {
        public float DizzyTime;
        public override TaskStatus OnUpdate()
        {
            if (enemy.CheckDizzyState())
            {

                ParticleManager.Instance.PlayEffect("Dizzy_star", enemy.transform.position + Vector3.up,
                    Quaternion.identity, enemy.transform, true, DizzyTime);
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
