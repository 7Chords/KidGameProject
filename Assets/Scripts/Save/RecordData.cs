using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class RecordData : SingletonPersistent<RecordData>
    {
        public const int recordNum = 3; //档位数
        public const string NAME = "RecordData"; //存档列表名
        
        // 游戏外数据字段
        public List<string> unlockedItems = new List<string>(); // 已解锁物品

        public string[] recordName = new string[recordNum]; //存档文件名
        public int lastID; //最新存档序号(用于重启时自动读档)

        class SaveData
        {
            public string[] recordName = new string[recordNum];
            public int lastID;
            
            // 游戏外数据
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
            
            // 游戏外数据保存
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


            // 游戏外数据加载
            unlockedItems = savedata.unlockedItems ?? new List<string>();
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

        // 更新游戏外数据
        public void UpdateGlobalData()
        {
            // 自动保存
            Save();
        }
    }
}