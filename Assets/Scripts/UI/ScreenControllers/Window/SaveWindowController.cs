using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;
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

        //�浵�����ʱִ��
        public static System.Action<int> OnLoad;

        private void Start()
        {
            //����ָ��������λ   
            for (int i = 0; i < RecordData.recordNum; i++)
            {
                GameObject obj = Instantiate(recordPrefab, grid);
                //�����
                obj.name = (i + 1).ToString();
                obj.GetComponent<SaveCell>().SetID(i + 1);
                //����õ�λ�д浵���͸�����Ĭ����Ϊ�յ�
                if (RecordData.Instance.recordName[i] != "")
                    obj.GetComponent<SaveCell>().SetName(i);
            }


            SaveCell.OnLeftClick += LeftClickGrid;
            SaveCell.OnEnter += ShowDetails;
            SaveCell.OnExit += HideDetails;
        }

        private void OnDestroy()
        {
            SaveCell.OnLeftClick -= LeftClickGrid;
            SaveCell.OnEnter -= ShowDetails;
            SaveCell.OnExit -= HideDetails;
        }

        //RecordUI.OnEnter����
        void ShowDetails(int i)
        {
            //��ȡ�浵�������޸�������ݣ���������ʾ
            var data = PlayerSaveData.Instance.ReadForShow(i);
            gameTime.text = $"��Ϸʱ��  {TimeMgr.GetFormatTime((int)data.gameTime)}";
            sceneName.text = $"���ڳ���  {data.scensName}";

            //��ʾ����
            detail.SetActive(true);
        }

        //RecordUI.OnExit����
        void HideDetails()
        {
            //��������
            detail.SetActive(false);
        }

        //������ش浵
        void LeftClickGrid(int gridID)
        {
            //�յ�ʲô������
            if (RecordData.Instance.recordName[gridID] == "")
                return;
            else
            {
                if (OnLoad != null)
                    OnLoad(gridID);
            }
        }
    }
}