

using BehaviorDesigner.Runtime.Tasks;

namespace KidGame.Core
{
    public class CanHearPlayer : BaseEnemyConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (enemy.PlayerInHearing())
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
