using BehaviorDesigner.Runtime.Tasks;


namespace KidGame.Core
{
    public class CheckAllRooms : BaseEnemyConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (enemy.CheckAllRooms()) return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}