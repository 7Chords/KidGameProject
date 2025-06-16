using UnityEngine;

namespace KidGame.Core
{
    public class RecordData : SingletonPersistent<RecordData>
    {
        public const int recordNum = 3; //档位数
        public const string NAME = "RecordData"; //存档列表名

        public string[] recordName = new string[recordNum]; //存档文件名(不是全路径名)
        public int lastID; //最新存档序号(用于重启时自动读档)

        class SaveData
        {
            public string[] recordName = new string[recordNum];
            public int lastID;
        }

        SaveData ForSave()
        {
            var savedata = new SaveData();

            for (int i = 0; i < recordNum; i++)
            {
                savedata.recordName[i] = recordName[i];
            }

            savedata.lastID = lastID;

            return savedata;
        }

        void ForLoad(SaveData savedata)
        {
            lastID = savedata.lastID;
            for (int i = 0; i < recordNum; i++)
            {
                recordName[i] = savedata.recordName[i];
            }
        }

        public void Save()
        {
            SAVE.PlayerPrefsSave(NAME, ForSave());
        }

        public void Load()
        {
            //有存档才读
            if (PlayerPrefs.HasKey(NAME))
            {
                string json = SAVE.PlayerPrefsLoad(NAME);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                ForLoad(saveData);
            }
        }
    }
}