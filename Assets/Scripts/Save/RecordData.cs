using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 存档记录有关，UI层控制器
    /// </summary>
    public class RecordData : SingletonPersistent<RecordData>
    {
        public const int recordNum = 3;
        public const string NAME = "RecordData";
        
        public string[] recordName = new string[recordNum];
        public int lastID;
        public List<string> unlockedItems = new List<string>();

        [System.Serializable]
        class SaveData
        {
            public string[] recordName = new string[recordNum];
            public int lastID;
            public List<string> unlockedItems;
        }

        SaveData ForSave()
        {
            var savedata = new SaveData();

            for (int i = 0; i < recordNum; i++)
            {
                savedata.recordName[i] = recordName[i] ?? string.Empty;
            }

            savedata.lastID = lastID;
            savedata.unlockedItems = unlockedItems ?? new List<string>();

            return savedata;
        }

        void ForLoad(SaveData savedata)
        {
            if (savedata == null) return;

            lastID = savedata.lastID;
            for (int i = 0; i < recordNum; i++)
            {
                recordName[i] = savedata.recordName[i] ?? $"Save_{i}";
            }
            
            unlockedItems = savedata.unlockedItems ?? new List<string>();
        }

        public void Save(bool encrypt = true)
        {
            try
            {
                SAVE.PlayerPrefsSave(NAME, ForSave(), encrypt);
                Debug.Log($"全局数据保存成功 - 加密: {encrypt}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"RecordData Save failed: {ex.Message}");
            }
        }

        public void Load(bool encrypted = true)
        {
            try
            {
                if (PlayerPrefs.HasKey(NAME))
                {
                    var saveData = SAVE.PlayerPrefsLoad<SaveData>(NAME, encrypted);
                    if (saveData != null)
                    {
                        ForLoad(saveData);
                        Debug.Log($"全局数据载入成功 - 加密: {encrypted}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"RecordData Load failed: {ex.Message}");
            }
        }

        public void UpdateGlobalData(bool encrypt = true)
        {
            Save(encrypt);
        }
    }
}