namespace UIFramework
{
    /// <summary>
    /// 窗口管理类
    /// </summary>
    public abstract class WindowController : WindowController<WindowProperties> { }

    /// <summary>
    /// 窗口管理基类
    /// </summary>
    public abstract class WindowController<TProps> : UIScreenController<TProps>, IWindowController
        where TProps : IWindowProperties
    {
        public bool HideOnForegroundLost {
            get { return Properties.HideOnForegroundLost; }
        }

        public bool IsPopup {
            get { return Properties.IsPopup; }
        }

        public WindowPriority WindowPriority {
            get { return Properties.WindowQueuePriority; }
        }

        /// <summary>
        /// 关闭窗口，使用 UI_ 前缀来方便找到对应的方法
        /// </summary>
        public virtual void UI_Close() {
            CloseRequest(this);
        }
        
        protected sealed override void SetProperties(TProps props) {
            if (props != null) {
                if (!props.SuppressPrefabProperties) {
                    props.HideOnForegroundLost = Properties.HideOnForegroundLost;
                    props.WindowQueuePriority = Properties.WindowQueuePriority;
                    props.IsPopup = Properties.IsPopup;
                }

                Properties = props;
            }
        }
        /// <summary>
        /// 把这个面板移动到最父物体的所有子物体中的最下面
        /// </summary>
        protected override void HierarchyFixOnShow() {
            transform.SetAsLastSibling();
            
        }
    }
}
