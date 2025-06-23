using KidGame.Core;
using UnityEngine.SceneManagement;
using Utils;

namespace KidGame.UI.Game
{
    public class StartGameSignal : ASignal
    {
    }

    public class ContinueGameSignal : ASignal
    {
    }

    public class ToSettingsWindowSignal : ASignal
    {
    }

    public class ExitGameSignal : ASignal
    {
    }

    public class StartWindowController : WindowController
    {
        public void UI_Start()
        {
            SceneLoader.Instance.LoadSceneWithTransition("GameScene",UIController.Instance.UICameraBindingVertexCamera);
            Signals.Get<StartGameSignal>().Dispatch();
            Hide();
        }

        public void UI_Continue()
        {
            Signals.Get<ContinueGameSignal>().Dispatch();
        }

        public void UI_Setting()
        {
            Signals.Get<ToSettingsWindowSignal>().Dispatch();
        }

        public void UI_Exit()
        {
            Signals.Get<ExitGameSignal>().Dispatch();
        }
    }
}