namespace KidGame.UI
{
    /// <summary>
    /// 面板控制器
    /// </summary>
    public abstract class PanelController : APanelController<PanelProperties>
    {
    }

    /// <summary>
    /// 面板控制器基类
    /// </summary>
    public abstract class APanelController<T> : UIScreenController<T>, IPanelController where T : IPanelProperties
    {
        public PanelPriority Priority
        {
            get
            {
                if (Properties != null)
                {
                    return Properties.Priority;
                }
                else
                {
                    return PanelPriority.None;
                }
            }
        }

        protected sealed override void SetProperties(T props)
        {
            base.SetProperties(props);
        }
    }
}