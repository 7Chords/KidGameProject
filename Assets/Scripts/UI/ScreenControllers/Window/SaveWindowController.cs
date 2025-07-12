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
        public Transform grid; //��λ������
        public GameObject recordPrefab; //��λԤ����

        [Header("�浵����")] 
        public GameObject detail; //�浵����
        public Text gameTime; //ʱ��
        public Text sceneName; //���ڳ���
        public Button loadButton; //��ȡ��ť
        public Button deleteButton; //ɾ����ť

        private int currentSelectedID = -1; //��ǰѡ�еĵ�λID

        private void Start()
        {
            //����ָ��������λ   
            for (int i = 0; i < RecordData.recordNum; i++)
            {
                GameObject obj = Instantiate(recordPrefab, grid);
                obj.name = (i + 1).ToString();
                obj.GetComponent<SaveCell>().SetID(i + 1);
                //����õ�λ�д浵���͸�����Ĭ����Ϊ�յ�
                if (RecordData.Instance.recordName[i] != "")
                    obj.GetComponent<SaveCell>().SetName(i);
            }

            SaveCell.OnLeftClick += LeftClickGrid;
            SaveCell.OnEnter += ShowDetails;
            SaveCell.OnExit += HideDetails;
            
            loadButton.onClick.AddListener(OnLoadButtonClick);
            deleteButton.onClick.AddListener(OnDeleteButtonClick);
        }

        private void OnDestroy()
        {
            SaveCell.OnLeftClick -= LeftClickGrid;
            SaveCell.OnEnter -= ShowDetails;
            SaveCell.OnExit -= HideDetails;
            
            loadButton.onClick.RemoveListener(OnLoadButtonClick);
            deleteButton.onClick.RemoveListener(OnDeleteButtonClick);
        }

        void ShowDetails(int i)
        {
            currentSelectedID = i;
            
            var data = PlayerSaveData.Instance.ReadForShow(i);
            gameTime.text = $"��Ϸʱ��  {TimeMgr.GetFormatTime((int)data.gameTime)}";
            sceneName.text = $"���ڳ���  {data.scensName}";

            // ֻ���д浵ʱ����ʾ��ť
            bool hasSave = RecordData.Instance.recordName[i] != "";
            loadButton.gameObject.SetActive(hasSave);
            deleteButton.gameObject.SetActive(hasSave);

            detail.SetActive(true);
        }

        void HideDetails()
        {
            currentSelectedID = -1;
            detail.SetActive(false);
        }

        // ��ȡ��ť����¼�
        void OnLoadButtonClick()
        {
            if (currentSelectedID != -1)
            {
                LoadRecord(currentSelectedID);
            }
        }

        // ɾ����ť����¼�
        void OnDeleteButtonClick()
        {
            if (currentSelectedID != -1)
            {
                // ɾ���浵
                RecordData.Instance.recordName[currentSelectedID] = "";
                PlayerSaveData.Instance.Delete(currentSelectedID);
                
                // ����UI
                Transform cell = grid.GetChild(currentSelectedID);
                cell.GetComponent<SaveCell>().SetName(currentSelectedID);
                
                // ��������Ͱ�ť
                detail.SetActive(false);
                currentSelectedID = -1;
            }
        }
        
        void LeftClickGrid(int gridID)
        {
            currentSelectedID = gridID;
            ShowDetails(gridID);
        }

        void LoadRecord(int i)
        {
            //����ָ���浵����
            PlayerSaveData.Instance.Load(i);

            //������´浵����i���͸������´浵����ţ�������
            if (i != RecordData.Instance.lastID)
            {
                RecordData.Instance.lastID = i;
                RecordData.Instance.Save();
            }
            
            PlayerSaveData.Instance.currentSaveSlot = i;

            //��ת����
            SceneManager.LoadScene(PlayerSaveData.Instance.scensName);
        }
    }
}