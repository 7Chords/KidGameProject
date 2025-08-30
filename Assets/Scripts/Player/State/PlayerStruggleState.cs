using KidGame.UI;

namespace KidGame.Core
{
    /// <summary>
    /// Íæ¼ÒÕõÔú×´Ì¬
    /// </summary>
    public class PlayerStruggleState : PlayerStateBase
    {

        public override void Enter()
        {
            UIHelper.Instance.ShowCircleProgress(GlobalValue.CIRCLE_PROGRESS_STRUGGLE,
                CircleProgressType.Manual, player.gameObject);
        }
        public override void Update()
        {
            base.Update();
            if(player.InputSettings.GetStruggleDown())
            {
                player.Struggle();
            }
        }

        public override void Exit()
        {
            UIHelper.Instance.DestoryCircleProgress(GlobalValue.CIRCLE_PROGRESS_STRUGGLE);
        }
    }
}
