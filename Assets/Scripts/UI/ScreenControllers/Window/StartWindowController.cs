using KidGame.Core;
using KidGame.UI.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace KidGame.UI
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
            //初始化玩家数据
            //可以在Player里写个Init函数，也可以在预制体上直接设置

            //跳转至默认场景
            SceneLoader.Instance.LoadSceneWithTransition("GameScene",
                UIController.Instance.UICameraBindingVertexCamera);
            
            Hide();
        }

        public void UI_Continue()
        {
            // 读取最新存档
            // LoadRecord(RecordData.Instance.lastID);
            
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

        private void Start()
        {
            //读取存档列表
            RecordData.Instance.Load();
            
            if (RecordData.Instance.lastID != 233)
            {
                //有存档才激活按钮
            }
        }
    }
}