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
        public Transform grid; // �浵��λ������
        public GameObject recordPrefab; // �浵��λԤ����
        public GameObject loadingPanel; // ������ʾ���
        public Text loadingText; // ������ʾ�ı�

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
        }

        private void LoadRecord(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= RecordData.recordNum)
            {
                Debug.LogError($"��Ч�Ĵ浵��λ����: {slotIndex}");
                return;
            }

            ShowLoadingPanel($"���ڼ��ش浵 {slotIndex + 1}...");

            // ���浵�Ƿ����
            if (string.IsNullOrEmpty(RecordData.Instance.recordName[slotIndex]))
            {
                Debug.LogWarning($"�浵��λ {slotIndex} û�д浵����");
                HideLoadingPanel();
                
                // û�еĻ������´浵
                CreateNewGame(slotIndex);
                return;
            }

            // ʹ��Э�̼��أ����⿨��
            StartCoroutine(LoadRecordCoroutine(slotIndex));
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

        // ��������Ϸ
        private void CreateNewGame(int slotIndex)
        {
            Debug.Log($"�ڲ�λ {slotIndex} ��������Ϸ");
            
            // ��ʼ������Ϸ����
            // PlayerSaveData.Instance.InitializeNewGame(slotIndex);
            // RecordData.Instance.recordName[slotIndex] = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            // RecordData.Instance.Save();
            
            // ������Ϸ
            // LoadRecord(slotIndex);
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
    }
}