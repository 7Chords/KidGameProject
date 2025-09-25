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
        public Transform grid; // �浵��λ������
        public GameObject recordPrefab; // �浵��λԤ����
        public GameObject loadingPanel; // ������ʾ���
        public Text loadingText; // ������ʾ�ı�

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
            // ������еĴ浵��λ
            foreach (Transform child in grid)
            {
                Destroy(child.gameObject);
            }

            saveCells.Clear();

            // ����ָ�������浵��λ
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

        #region ɾ���浵

                private void OnSaveCellDeleteClicked(int slotIndex)
                {
                    // ��λ����û������Ϊ�޵���ˢ��һ�� UI ����
                    if (string.IsNullOrEmpty(RecordData.Instance.recordName[slotIndex]))
                    {
                        if (slotIndex >= 0 && slotIndex < saveCells.Count) 
                            saveCells[slotIndex].RefreshDisplay();
                        return;
                    }
                    
                    var props = new ConfirmationPopupProperties(
                        "ɾ���浵",
                        $"ȷ��Ҫɾ����λ {slotIndex + 1} �Ĵ浵�𣿸ò������ɳ�����",
                        "ȷ��", () => ConfirmDelete(slotIndex),
                        "ȡ��", () => { /* no-op */ }
                    );
        
                    Signals.Get<ShowConfirmationPopupSignal>().Dispatch(props);
                }
        
                private void ConfirmDelete(int slotIndex)
                {
                    // 1) ɾ�ļ�
                    PlayerSaveData.Instance.Delete(slotIndex);
        
                    // 2) ��ռ�¼�������� lastID
                    if (RecordData.Instance.lastID == slotIndex)
                    {
                        RecordData.Instance.lastID = FindAnyExistingSaveSlotOrMinusOne(slotIndex);
                    }
                    RecordData.Instance.recordName[slotIndex] = null;
                    RecordData.Instance.Save();
        
                    // 3) ˢ�¸ø���ʾ
                    if (slotIndex >= 0 && slotIndex < saveCells.Count)
                    {
                        saveCells[slotIndex].Initialize(slotIndex);
                    }
        
                    Debug.Log($"��ɾ���浵�� {slotIndex}");
                }
                
                private int FindAnyExistingSaveSlotOrMinusOne(int preferExclude)
                {
                    for (int i = 0; i < RecordData.recordNum; i++)
                    {
                        if (i == preferExclude) continue;
                        var name = RecordData.Instance.recordName[i];
                        if (!string.IsNullOrEmpty(name))
                        {
                            // ��һ��ȷ���ļ���Ĵ���
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
                        Debug.LogError($"��Ч�Ĵ浵��λ����: {slotIndex}");
                        return;
                    }
        
                    ShowLoadingPanel($"���ڼ��ش浵 {slotIndex + 1}...");
        
                    // 1) ��λ��Ϊ�գ��½��浵
                    if (string.IsNullOrEmpty(RecordData.Instance.recordName[slotIndex]))
                    {
                        HideLoadingPanel();
                        CreateNewGame(slotIndex);
                        return;
                    }
        
                    // 2) �����ֵ��ļ����ܲ����ڻ򻵵�
                    var preview = PlayerSaveData.Instance.ReadForShow(slotIndex);
                    if (preview == null)
                    {
                        HideLoadingPanel();
                        CreateNewGame(slotIndex);
                        return;
                    }
        
                    // ������ȡ
                    StartCoroutine(LoadRecordCoroutine(slotIndex));
                }

        #endregion
        
        private void CreateNewGame(int slotIndex)
        {
            Debug.Log($"�ڲ�λ {slotIndex} ��������Ϸ");
            
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
            // �����ӳ٣���UI����Ӧʱ��
            yield return new WaitForSeconds(0.1f);

            try
            {
                // ����ָ���浵����
                PlayerSaveData.Instance.Load(slotIndex);

                // �������ʹ�õĴ浵���
                if (slotIndex != RecordData.Instance.lastID)
                {
                    RecordData.Instance.lastID = slotIndex;
                    RecordData.Instance.Save();
                }

                PlayerSaveData.Instance.currentSaveSlot = slotIndex;

                UpdateLoadingText("���볡��...");

                // ��ת����
                if (!string.IsNullOrEmpty(PlayerSaveData.Instance.scensName))
                {
                    SceneManager.LoadScene(PlayerSaveData.Instance.scensName);
                }
                else
                {
                    Debug.LogError("�浵�г�������Ϊ�գ���ת��Ĭ�ϳ���");
                    SceneManager.LoadScene("MainMenu");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"���ش浵ʧ��: {ex.Message}");
                UpdateLoadingText($"����ʧ��: {ex.Message}");
                HideLoadingPanel();
            }
        }

        private void ShowLoadingPanel(string message = "������...")
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