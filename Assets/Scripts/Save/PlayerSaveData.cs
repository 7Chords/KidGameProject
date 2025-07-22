using UnityEngine;
using System.Collections.Generic;


namespace KidGame.Core
{
    public class PlayerSaveData : SingletonPersistent<PlayerSaveData>
    {
        public int level; // 当前等级
        public string scensName; // 场景名称  
        public float gameTime; // 游戏时间
        public int currentSaveSlot = -1; // 当前存档编号(-1表示未存档)
        
        // 背包数据
        public List<TrapSlotInfo> trapBag = new List<TrapSlotInfo>();
        public List<MaterialSlotInfo> materialBag = new List<MaterialSlotInfo>();
        
        // 当前天数
        public int currentDay;

        [System.Serializable]
        public class SaveData
        {
            public string scensName;
            public int level;
            public float gameTime;
            public int currentSaveSlot;
            
            public List<TrapSlotInfo> trapBag;
            public List<MaterialSlotInfo> materialBag;
            public int currentDay;
        }

        SaveData ForSave()
        {
            var savedata = new SaveData();
            savedata.scensName = scensName;
            savedata.level = level;
            savedata.gameTime = gameTime;
            savedata.currentSaveSlot = currentSaveSlot;

            // 保存数据
            if (PlayerBag.Instance != null)
            {
                //todo :guihuala
                //savedata.trapBag = PlayerBag.Instance._trapBag;
                //savedata.materialBag = PlayerBag.Instance._materialBag;
            }
            if (GameLevelManager.Instance != null)
                savedata.currentDay = GameLevelManager.Instance.GetCurrentDay();
            
            return savedata;
        }

        void ForLoad(SaveData savedata)
        {
            scensName = savedata.scensName;
            level = savedata.level;
            gameTime = savedata.gameTime;
            currentSaveSlot = savedata.currentSaveSlot;
            
            trapBag = savedata.trapBag ?? new List<TrapSlotInfo>();
            materialBag = savedata.materialBag ?? new List<MaterialSlotInfo>();
            currentDay = savedata.currentDay;
        }

        public void Save(int id)
        {
            // 更新当前存档编号
            currentSaveSlot = id;
            
            RecordData.Instance.UpdateGlobalData();
            
            SAVE.JsonSave(RecordData.Instance.recordName[id], ForSave());
        }

        // 自动保存到当前存档
        public void AutoSave()
        {
            if (currentSaveSlot >= 0)
            {
                Save(currentSaveSlot);
            }
        }

        public void Load(int id)
        {
            var saveData = SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id]);
            if (saveData != null)
            {
                ForLoad(saveData);
                
                // 加载后更新数据
                // PlayerBag.Instance.LoadBagData(trapBag, materialBag);
                GameLevelManager.Instance.SetCurrentDay(currentDay);
                
                // 设置当前存档编号
                currentSaveSlot = id;
            }
        }

        public SaveData ReadForShow(int id)
        {
            return SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id]);
        }

        public void Delete(int id)
        {
            SAVE.JsonDelete(RecordData.Instance.recordName[id]);
            if (currentSaveSlot == id)
            {
                currentSaveSlot = -1;
            }
        }
    }
}