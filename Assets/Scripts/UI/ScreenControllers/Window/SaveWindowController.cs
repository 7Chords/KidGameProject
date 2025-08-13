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

        private int currentSelectedID = -1; //��ǰѡ�еĵ�λID

        private void Start()
        {
            //����ָ��������λ   
            for (int i = 0; i < RecordData.recordNum; i++)
            {
                GameObject obj = Instantiate(recordPrefab, grid);
                obj.name = (i + 1).ToString();
                obj.GetComponent<SaveCell>().SetName(i + 1);
                //����õ�λ�д浵���͸�����Ĭ����Ϊ�յ�
                if (RecordData.Instance.recordName[i] != "")
                    obj.GetComponent<SaveCell>().SetName(i);
            }

            SaveCell.OnLeftClick += LeftClickGrid;
        }

        private void OnDestroy()
        {
            SaveCell.OnLeftClick -= LeftClickGrid;
        }
        

        // ��ȡ��ť����¼�
        void OnLoadButtonClick()
        {
            if (currentSelectedID != -1)
            {
                LoadRecord(currentSelectedID);
            }
        }
        
        void LeftClickGrid(int gridID)
        {
            currentSelectedID = gridID;
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