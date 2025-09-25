using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;


namespace KidGame.UI
{
    public class SaveWindowController : WindowController
    {
        public Transform grid; // 存档槽位父对象
        public GameObject recordPrefab; // 存档槽位预制体
        public GameObject loadingPanel; // 加载提示面板
        public Text loadingText; // 加载提示文本

        private List<SaveCell> saveCells = new List<SaveCell>();

        private void Start()
        {
            InitializeSaveSlots();

            SaveCell.OnLeftClick += OnSaveCellClicked;
            SaveCell.OnDeleteClick += OnSaveCellDeleteClicked;
        }

        private void OnDestroy()
        {
            SaveCell.OnLeftClick -= OnSaveCellClicked;
            SaveCell.OnDeleteClick -= OnSaveCellDeleteClicked;
        }

        private void InitializeSaveSlots()
        {
            // 清空现有的存档槽位
            foreach (Transform child in grid)
            {
                Destroy(child.gameObject);
            }

            saveCells.Clear();

            // 生成指定数量存档槽位
            for (int i = 0; i < RecordData.recordNum; i++)
            {
                GameObject obj = Instantiate(recordPrefab, grid);
                obj.name = $"SaveSlot_{i}";

                SaveCell cell = obj.GetComponent<SaveCell>();
                if (cell != null)
                {
                    cell.Initialize(i);
                    saveCells.Add(cell);
                }
            }
        }

        private void OnSaveCellClicked(int slotIndex)
        {
            LoadRecord(slotIndex);
            Hide();
        }

        #region 删除存档

                private void OnSaveCellDeleteClicked(int slotIndex)
                {
                    // 槽位本就没名，视为无档，刷新一下 UI 即可
                    if (string.IsNullOrEmpty(RecordData.Instance.recordName[slotIndex]))
                    {
                        if (slotIndex >= 0 && slotIndex < saveCells.Count) 
                            saveCells[slotIndex].RefreshDisplay();
                        return;
                    }
                    
                    var props = new ConfirmationPopupProperties(
                        "删除存档",
                        $"确定要删除槽位 {slotIndex + 1} 的存档吗？该操作不可撤销。",
                        "确认", () => ConfirmDelete(slotIndex),
                        "取消", () => { /* no-op */ }
                    );
        
                    Signals.Get<ShowConfirmationPopupSignal>().Dispatch(props);
                }
        
                private void ConfirmDelete(int slotIndex)
                {
                    // 1) 删文件
                    PlayerSaveData.Instance.Delete(slotIndex);
        
                    // 2) 清空记录名，修正 lastID
                    if (RecordData.Instance.lastID == slotIndex)
                    {
                        RecordData.Instance.lastID = FindAnyExistingSaveSlotOrMinusOne(slotIndex);
                    }
                    RecordData.Instance.recordName[slotIndex] = null;
                    RecordData.Instance.Save();
        
                    // 3) 刷新该格显示
                    if (slotIndex >= 0 && slotIndex < saveCells.Count)
                    {
                        saveCells[slotIndex].Initialize(slotIndex);
                    }
        
                    Debug.Log($"已删除存档槽 {slotIndex}");
                }
                
                private int FindAnyExistingSaveSlotOrMinusOne(int preferExclude)
                {
                    for (int i = 0; i < RecordData.recordNum; i++)
                    {
                        if (i == preferExclude) continue;
                        var name = RecordData.Instance.recordName[i];
                        if (!string.IsNullOrEmpty(name))
                        {
                            // 进一步确认文件真的存在
                            var preview = PlayerSaveData.Instance.ReadForShow(i);
                            if (preview != null) return i;
                        }
                    }
                    return -1;
                }
        
                private void LoadRecord(int slotIndex)
                {
                    if (slotIndex < 0 || slotIndex >= RecordData.recordNum)
                    {
                        Debug.LogError($"无效的存档槽位索引: {slotIndex}");
                        return;
                    }
        
                    ShowLoadingPanel($"正在加载存档 {slotIndex + 1}...");
        
                    // 1) 槽位名为空，新建存档
                    if (string.IsNullOrEmpty(RecordData.Instance.recordName[slotIndex]))
                    {
                        HideLoadingPanel();
                        CreateNewGame(slotIndex);
                        return;
                    }
        
                    // 2) 有名字但文件可能不存在或坏档
                    var preview = PlayerSaveData.Instance.ReadForShow(slotIndex);
                    if (preview == null)
                    {
                        HideLoadingPanel();
                        CreateNewGame(slotIndex);
                        return;
                    }
        
                    // 正常读取
                    StartCoroutine(LoadRecordCoroutine(slotIndex));
                }

        #endregion
        
        private void CreateNewGame(int slotIndex)
        {
            Debug.Log($"在槽位 {slotIndex} 创建新游戏");
            
            string saveName = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            RecordData.Instance.recordName[slotIndex] = saveName;
            RecordData.Instance.lastID = slotIndex;
            RecordData.Instance.Save();

            PlayerSaveData.Instance.InitializeNewGame(slotIndex, "GameScene");
            PlayerSaveData.Instance.Save(slotIndex);
            
            SceneManager.LoadScene("GameScene");
        }

        private IEnumerator LoadRecordCoroutine(int slotIndex)
        {
            // 短暂延迟，让UI有响应时间
            yield return new WaitForSeconds(0.1f);

            try
            {
                // 载入指定存档数据
                PlayerSaveData.Instance.Load(slotIndex);

                // 更新最后使用的存档序号
                if (slotIndex != RecordData.Instance.lastID)
                {
                    RecordData.Instance.lastID = slotIndex;
                    RecordData.Instance.Save();
                }

                PlayerSaveData.Instance.currentSaveSlot = slotIndex;

                UpdateLoadingText("载入场景...");

                // 跳转场景
                if (!string.IsNullOrEmpty(PlayerSaveData.Instance.scensName))
                {
                    SceneManager.LoadScene(PlayerSaveData.Instance.scensName);
                }
                else
                {
                    Debug.LogError("存档中场景名称为空，跳转到默认场景");
                    SceneManager.LoadScene("MainMenu");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"加载存档失败: {ex.Message}");
                UpdateLoadingText($"加载失败: {ex.Message}");
                HideLoadingPanel();
            }
        }

        private void ShowLoadingPanel(string message = "加载中...")
        {
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(true);
            }

            if (loadingText != null)
            {
                loadingText.text = message;
            }
        }

        private void UpdateLoadingText(string message)
        {
            if (loadingText != null)
            {
                loadingText.text = message;
            }
        }

        private void HideLoadingPanel()
        {
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }
        }
        
        public void UI_Close()
        {
            Hide();
        }
    }
}