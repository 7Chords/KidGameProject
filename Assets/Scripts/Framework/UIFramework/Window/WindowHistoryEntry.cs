namespace KidGame.UI {
    /// <summary>
    /// 窗口记录和队列
    /// </summary>
    public struct WindowHistoryEntry
    {
        public readonly IWindowController Screen;
        public readonly IWindowProperties Properties;

        public WindowHistoryEntry(IWindowController screen, IWindowProperties properties) {
            Screen = screen;
            Properties = properties;
        }

        public void Show() {
            Screen.Show(Properties);
        }
    }
}
