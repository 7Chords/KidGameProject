

using BehaviorDesigner.Runtime.Tasks;
using KidGame.UI;
using UnityEngine;

namespace KidGame.Core
{
    public class CanHearPlayer : BaseEnemyConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (enemy.PlayerInHearing())
            {
                ParticleManager.Instance.PlayEffect("Attention_spark", enemy.transform.position + Vector3.up,
                    Quaternion.identity, enemy.transform, true, 0.5f);
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
