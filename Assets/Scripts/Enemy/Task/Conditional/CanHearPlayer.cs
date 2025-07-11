

using BehaviorDesigner.Runtime.Tasks;
using KidGame.UI;

namespace KidGame.Core
{
    public class CanHearPlayer : BaseEnemyConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (enemy.PlayerInHearing())
            {
                UIHelper.Instance.ShowOneSign(new SignInfo(GlobalValue.SIGN_ICON_ATTENTION_PATH,enemy.gameObject));
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
