using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        }

        private void OnDestroy()
        {
            SaveCell.OnLeftClick -= OnSaveCellClicked;
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
        }

        private void LoadRecord(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= RecordData.recordNum)
            {
                Debug.LogError($"无效的存档槽位索引: {slotIndex}");
                return;
            }

            ShowLoadingPanel($"正在加载存档 {slotIndex + 1}...");

            // 检查存档是否存在
            if (string.IsNullOrEmpty(RecordData.Instance.recordName[slotIndex]))
            {
                Debug.LogWarning($"存档槽位 {slotIndex} 没有存档数据");
                HideLoadingPanel();
                
                // 没有的话创建新存档
                CreateNewGame(slotIndex);
                return;
            }

            // 使用协程加载，避免卡顿
            StartCoroutine(LoadRecordCoroutine(slotIndex));
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

        // 创建新游戏
        private void CreateNewGame(int slotIndex)
        {
            Debug.Log($"在槽位 {slotIndex} 创建新游戏");
            
            // 初始化新游戏数据
            // PlayerSaveData.Instance.InitializeNewGame(slotIndex);
            // RecordData.Instance.recordName[slotIndex] = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            // RecordData.Instance.Save();
            
            // 加载游戏
            // LoadRecord(slotIndex);
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
    }
}