namespace KidGame.Core
{
    /// <summary>
    /// Íæ¼ÒÕõÔú×´Ì¬
    /// </summary>
    public class PlayerStruggleState : PlayerStateBase
    {
        public override void Update()
        {
            base.Update();
            if(player.InputSettings.GetStruggleDown())
            {
                player.Struggle();
            }
        }
    }
}
