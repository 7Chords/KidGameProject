namespace KidGame.Interface
{
    /// <summary>
    /// 可以互动的接口
    /// </summary>
    public interface IInteractive
    {
        //主动互动
        public abstract void InteractPositive();

        //被动互动
        public abstract void InteractNegative();
    }
}