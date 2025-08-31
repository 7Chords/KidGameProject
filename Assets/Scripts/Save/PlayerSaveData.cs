using UnityEngine;
using System.Collections.Generic;
using System;

namespace KidGame.Core
{
    /// <summary>
    /// �浵������
    /// </summary>
    public class PlayerSaveData : SingletonPersistent<PlayerSaveData>
    {
        public int level;
        public string scensName;
        public float gameTime;
        public int currentSaveSlot = -1;
        
        public List<TrapSlotInfo> trapBag = new List<TrapSlotInfo>();
        public List<MaterialSlotInfo> materialBag = new List<MaterialSlotInfo>();
        public int currentDay;

        // Ԫ�����ֶ�
        public string gameVersion;
        public long lastSavedTimestamp;
        public string playerName;
        public int totalPlayTimeSeconds;
        public string saveThumbnailData; // Base64���������ͼ

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

            // Ԫ����
            public string gameVersion;
            public long lastSavedTimestamp;
            public string playerName;
            public int totalPlayTimeSeconds;
            public string saveThumbnailData;
        }

        #region �洢���ȡ��˽�У�

        SaveData ForSave()
        {
            var savedata = new SaveData();
            savedata.scensName = scensName;
            savedata.level = level;
            savedata.gameTime = gameTime;
            savedata.currentSaveSlot = currentSaveSlot;

            if (PlayerBag.Instance != null)
            {
                // ��PlayerBag��ȡ��������
            }
            else
            {
                savedata.trapBag = new List<TrapSlotInfo>(trapBag);
                savedata.materialBag = new List<MaterialSlotInfo>(materialBag);
            }

            if (GameLevelManager.Instance != null)
                savedata.currentDay = GameLevelManager.Instance.GetCurrentDay();
            else
                savedata.currentDay = currentDay;

            savedata.gameVersion = Application.version;
            savedata.lastSavedTimestamp = DateTime.UtcNow.Ticks;
            savedata.playerName = playerName;
            savedata.totalPlayTimeSeconds = totalPlayTimeSeconds;
            savedata.saveThumbnailData = saveThumbnailData;

            return savedata;
        }

        void ForLoad(SaveData savedata)
        {
            if (savedata == null)
            {
                Debug.LogWarning("Attempted to load null save data");
                return;
            }

            scensName = savedata.scensName ?? string.Empty;
            level = savedata.level;
            gameTime = savedata.gameTime;
            currentSaveSlot = savedata.currentSaveSlot;

            trapBag = savedata.trapBag ?? new List<TrapSlotInfo>();
            materialBag = savedata.materialBag ?? new List<MaterialSlotInfo>();
            currentDay = savedata.currentDay;

            // ����Ԫ����
            gameVersion = savedata.gameVersion;
            lastSavedTimestamp = savedata.lastSavedTimestamp;
            playerName = savedata.playerName;
            totalPlayTimeSeconds = savedata.totalPlayTimeSeconds;
            saveThumbnailData = savedata.saveThumbnailData;
        }

        #endregion

        /// <summary>
        /// �ֶ�����
        /// </summary>
        /// <param name="id">�浵���</param>
        /// <param name="encrypt">�Ƿ����</param>
        public void Save(int id, bool encrypt = true)
        {
            try
            {
                currentSaveSlot = id;
                RecordData.Instance.UpdateGlobalData();
                
                // ��������Ϸʱ��
                totalPlayTimeSeconds = Mathf.FloorToInt(gameTime);
                
                SAVE.JsonSave(RecordData.Instance.recordName[id], ForSave(), encrypt);
                
                Debug.Log($"�浵�ɹ� - ��λ: {id}, ����: {encrypt}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Save failed for slot {id}: {ex.Message}");
            }
        }

        /// <summary>
        /// �Զ�����
        /// </summary>
        /// <param name="encrypt">�Ƿ����</param>
        public void AutoSave(bool encrypt = true)
        {
            if (currentSaveSlot >= 0)
            {
                Save(currentSaveSlot, encrypt);
            }
        }

        public void Load(int id, bool encrypted = true)
        {
            try
            {
                var saveData = SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id], encrypted);
                if (saveData != null)
                {
                    ForLoad(saveData);
                    
                    if (PlayerBag.Instance != null)
                    {
                        PlayerBag.Instance.LoadBagData(trapBag, materialBag);
                    }
                    
                    if (GameLevelManager.Instance != null)
                    {
                        GameLevelManager.Instance.SetCurrentDay(currentDay);
                    }
                    
                    currentSaveSlot = id;
                    Debug.Log($"����ɹ� - ��λ: {id}, �汾: {gameVersion}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Load failed for slot {id}: {ex.Message}");
            }
        }

        public SaveData ReadForShow(int id, bool encrypted = true)
        {
            try
            {
                return SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id], encrypted);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"ReadForShow failed for slot {id}: {ex.Message}");
                return null;
            }
        }

        public void Delete(int id)
        {
            try
            {
                SAVE.JsonDelete(RecordData.Instance.recordName[id]);
                if (currentSaveSlot == id)
                {
                    currentSaveSlot = -1;
                }
                Debug.Log($"ɾ���浵 - ��λ: {id}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Delete failed for slot {id}: {ex.Message}");
            }
        }

        #region ��������

        // ��ȡ��ʽ��ʱ��
        public string GetFormattedPlayTime()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalPlayTimeSeconds);
            return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }

        // ��ȡ��󱣴�ʱ��
        public string GetFormattedLastSavedTime()
        {
            if (lastSavedTimestamp == 0) return "��δ����";
            DateTime saveTime = new DateTime(lastSavedTimestamp);
            return saveTime.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
        }

        // ��֤�浵������
        public bool ValidateSaveData(SaveData data)
        {
            if (data == null) return false;
            if (data.level < 0) return false;
            if (data.trapBag == null || data.materialBag == null) return false;
            if (string.IsNullOrEmpty(data.gameVersion)) return false;
            
            return true;
        }

        #endregion
    }
}