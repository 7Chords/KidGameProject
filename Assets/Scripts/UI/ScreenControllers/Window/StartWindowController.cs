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

            // 判定是否有“最近存档”
            bool hasRecent = false;
            int id = RecordData.Instance.lastID;
            if (id >= 0 && id < RecordData.recordNum && !string.IsNullOrEmpty(RecordData.Instance.recordName[id]))
            {
                // 确认文件存在且可解析
                var preview = PlayerSaveData.Instance.ReadForShow(id);
                hasRecent = (preview != null);
            }

            continueButton.gameObject.SetActive(hasRecent);
            startButton.onClick.AddListener(UI_Start);
            continueButton.onClick.AddListener(UI_Continue);
            settingsButton.onClick.AddListener(UI_Setting);
            exitButton.onClick.AddListener(UI_Exit);
        }

        public void UI_Start()
        {
            // 直接走“创建新游戏”路径（与存档面板一致的逻辑）
            int saveSlot = GetAvailableSaveSlot();
            string saveName = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            RecordData.Instance.recordName[saveSlot] = saveName;
            RecordData.Instance.lastID = saveSlot;
            RecordData.Instance.Save();

            PlayerSaveData.Instance.InitializeNewGame(saveSlot, "GameScene");
            PlayerSaveData.Instance.Save(saveSlot);

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