using UnityEngine;
using System.Collections.Generic;


namespace KidGame.Core
{
    public class PlayerSaveData : SingletonPersistent<PlayerSaveData>
    {
        public int level; // ��ǰ�ȼ�
        public string scensName; // ��������  
        public float gameTime; // ��Ϸʱ��
        public int currentSaveSlot = -1; // ��ǰ�浵���(-1��ʾδ�浵)
        
        // ��������
        public List<TrapSlotInfo> trapBag = new List<TrapSlotInfo>();
        public List<MaterialSlotInfo> materialBag = new List<MaterialSlotInfo>();
        
        // ��ǰ����
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

            // ��������
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
            // ���µ�ǰ�浵���
            currentSaveSlot = id;
            
            RecordData.Instance.UpdateGlobalData();
            
            SAVE.JsonSave(RecordData.Instance.recordName[id], ForSave());
        }

        // �Զ����浽��ǰ�浵
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
                
                // ���غ��������
                // PlayerBag.Instance.LoadBagData(trapBag, materialBag);
                GameLevelManager.Instance.SetCurrentDay(currentDay);
                
                // ���õ�ǰ�浵���
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