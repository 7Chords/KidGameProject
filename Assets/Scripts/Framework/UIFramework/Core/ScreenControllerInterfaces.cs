using System;

namespace KidGame.UI {
    /// <summary>
    /// 所有的UI界面必须实现的接口，统一风格
    /// </summary>
    public interface IScreenController {
        string ScreenId { get; set; }
        bool IsVisible { get; }

        void Show(IScreenProperties props = null);
        void Hide(bool animate = true);

        Action<IScreenController> InTransitionFinished { get; set; }
        Action<IScreenController> OutTransitionFinished { get; set; }
        Action<IScreenController> CloseRequest { get; set; }
        Action<IScreenController> ScreenDestroyed { get; set; }
    }

    /// <summary>
    /// 所有的窗口必须实现的接口
    /// </summary>
    public interface IWindowController : IScreenController {
        //失去焦点时是否隐藏
        bool HideOnForegroundLost { get; }
        bool IsPopup { get; }
        WindowPriority WindowPriority { get; }
    }

    /// <summary>
    /// 所有的面板必须实现的接口
    /// </summary>
    public interface IPanelController : IScreenController {
        PanelPriority Priority { get; }
    }
}
