using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class RecordData : SingletonPersistent<RecordData>
    {
        public const int recordNum = 3; //��λ��
        public const string NAME = "RecordData"; //�浵�б���
        
        // ��Ϸ�������ֶ�
        public List<string> unlockedItems = new List<string>(); // �ѽ�����Ʒ

        public string[] recordName = new string[recordNum]; //�浵�ļ���
        public int lastID; //���´浵���(��������ʱ�Զ�����)

        class SaveData
        {
            public string[] recordName = new string[recordNum];
            public int lastID;
            
            // ��Ϸ������
            public List<string> unlockedItems;
        }

        SaveData ForSave()
        {
            var savedata = new SaveData();

            for (int i = 0; i < recordNum; i++)
            {
                savedata.recordName[i] = recordName[i];
            }

            savedata.lastID = lastID;
            
            // ��Ϸ�����ݱ���
            savedata.unlockedItems = unlockedItems;

            return savedata;
        }

        void ForLoad(SaveData savedata)
        {
            lastID = savedata.lastID;
            for (int i = 0; i < recordNum; i++)
            {
                recordName[i] = savedata.recordName[i];
            }


            // ��Ϸ�����ݼ���
            unlockedItems = savedata.unlockedItems ?? new List<string>();
        }

        public void Save()
        {
            SAVE.PlayerPrefsSave(NAME, ForSave());
        }

        public void Load()
        {
            //�д浵�Ŷ�
            if (PlayerPrefs.HasKey(NAME))
            {
                string json = SAVE.PlayerPrefsLoad(NAME);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                ForLoad(saveData);
            }
        }

        // ������Ϸ������
        public void UpdateGlobalData()
        {
            // �Զ�����
            Save();
        }
    }
}