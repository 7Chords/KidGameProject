using UnityEngine;
using System.Collections.Generic;

namespace KidGame.Core
{
    /// <summary>
    /// ��Ҵ浵����
    /// </summary>
    public class PlayerSaveData : SingletonPersistent<PlayerSaveData>
    {
        public int level; // ��ǰ�ȼ�
        public string scensName; // ��������  
        public float gameTime; // ��Ϸʱ��
        
        // ��������
        public List<TrapSlotInfo> trapBag = new List<TrapSlotInfo>();
        public List<MaterialSlotInfo> materialBag = new List<MaterialSlotInfo>();
        
        // ֻ���浱ǰ����
        public int currentDay;

        [System.Serializable]
        public class SaveData
        {
            public string scensName;
            public int level;
            public float gameTime;
            
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
            
            savedata.trapBag = trapBag;
            savedata.materialBag = materialBag;
            savedata.currentDay = GameLevelManager.Instance.GetCurrentDay();
            
            return savedata;
        }

        void ForLoad(SaveData savedata)
        {
            scensName = savedata.scensName;
            level = savedata.level;
            gameTime = savedata.gameTime;
            
            trapBag = savedata.trapBag ?? new List<TrapSlotInfo>();
            materialBag = savedata.materialBag ?? new List<MaterialSlotInfo>();
            currentDay = savedata.currentDay;
        }

        public void Save(int id)
        {
            // ����ǰ��������
            trapBag = PlayerBag.Instance.GetTrapSlots();
            materialBag = PlayerBag.Instance.GetMaterialSlots();
            
            RecordData.Instance.UpdateGlobalData();
            
            SAVE.JsonSave(RecordData.Instance.recordName[id], ForSave());
        }

        public void Load(int id)
        {
            var saveData = SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id]);
            if (saveData != null)
            {
                ForLoad(saveData);
                
                // ���غ��������
                PlayerBag.Instance.LoadBagData(trapBag, materialBag);
                GameLevelManager.Instance.SetCurrentDay(currentDay);
            }
        }

        public SaveData ReadForShow(int id)
        {
            return SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id]);
        }

        public void Delete(int id)
        {
            SAVE.JsonDelete(RecordData.Instance.recordName[id]);
        }
    }
}