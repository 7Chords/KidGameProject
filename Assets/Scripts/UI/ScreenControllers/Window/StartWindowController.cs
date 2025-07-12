using KidGame.Core;
using KidGame.UI.Game;
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
        public Button startButton;
        public Button continueButton;
        public Button settingsButton;
        public Button exitButton;
        
        private void Start()
        {
            //读取存档列表
            RecordData.Instance.Load();
            
            if (RecordData.Instance.lastID != 123)
            {
                continueButton.gameObject.SetActive(false);
            }
            
            startButton.onClick.AddListener(UI_Start);
            continueButton.onClick.AddListener(UI_Continue);
            settingsButton.onClick.AddListener(UI_Setting);
            exitButton.onClick.AddListener(UI_Exit);
        }
        
        public void UI_Start()
        {
            // 创建新存档
            int saveSlot = GetAvailableSaveSlot();
            
            string saveName = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            RecordData.Instance.recordName[saveSlot] = saveName;
            RecordData.Instance.lastID = saveSlot;
            RecordData.Instance.Save();
            
            PlayerSaveData.Instance.scensName = "GameScene";
            PlayerSaveData.Instance.level = 1;
            PlayerSaveData.Instance.gameTime = 0;
            PlayerSaveData.Instance.currentSaveSlot = saveSlot; // 设置当前存档编号
            PlayerSaveData.Instance.Save(saveSlot);
    
            // 跳转场景
            SceneLoader.Instance.LoadSceneWithTransition("GameScene",
                UIController.Instance.UICameraBindingVertexCamera);
            
            Hide();
        }

        private int GetAvailableSaveSlot()
        {
            // 检查是否有空存档位
            for (int i = 0; i < RecordData.recordNum; i++)
            {
                if (string.IsNullOrEmpty(RecordData.Instance.recordName[i]))
                {
                    return i;
                }
            }
            
            // 如果没有空位，默认覆盖第一个存档
            return 0;
        }

        public void UI_Continue()
        {
            // 读取最新存档
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