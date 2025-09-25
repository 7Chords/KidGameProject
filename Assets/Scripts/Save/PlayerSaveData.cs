using UnityEngine;
using System.Collections.Generic;
using System;

namespace KidGame.Core
{
    /// <summary>
    /// 存档控制器
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

        // 元数据字段
        public string gameVersion;
        public long lastSavedTimestamp;
        public string playerName;
        public int totalPlayTimeSeconds;
        public string saveThumbnailData; // Base64编码的缩略图

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

            // 元数据
            public string gameVersion;
            public long lastSavedTimestamp;
            public string playerName;
            public int totalPlayTimeSeconds;
            public string saveThumbnailData;
        }

        #region 存储与读取（私有）

        SaveData ForSave()
        {
            var savedata = new SaveData();
            savedata.scensName = scensName;
            savedata.level = level;
            savedata.gameTime = gameTime;
            savedata.currentSaveSlot = currentSaveSlot;

            if (PlayerBag.Instance != null)
            {
                // 从PlayerBag获取最新数据
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

            // 加载元数据
            gameVersion = savedata.gameVersion;
            lastSavedTimestamp = savedata.lastSavedTimestamp;
            playerName = savedata.playerName;
            totalPlayTimeSeconds = savedata.totalPlayTimeSeconds;
            saveThumbnailData = savedata.saveThumbnailData;
        }

        #endregion
        
        public void InitializeNewGame(int slotIndex, string firstSceneName = "GameScene")
        {
            level = 1;
            scensName = firstSceneName;
            gameTime = 0;
            currentDay = 1;
            trapBag = new List<TrapSlotInfo>();
            materialBag = new List<MaterialSlotInfo>();
            playerName = string.IsNullOrEmpty(playerName) ? "Player" : playerName;
            totalPlayTimeSeconds = 0;
            lastSavedTimestamp = DateTime.UtcNow.Ticks;
            currentSaveSlot = slotIndex;
        }
        
        /// <summary>
        /// 手动保存
        /// </summary>
        /// <param name="id">存档编号</param>
        /// <param name="encrypt">是否加密</param>
        public void Save(int id, bool encrypt = true)
        {
            try
            {
                currentSaveSlot = id;

                // ★ 兜底：槽位名为空则自动命名
                if (string.IsNullOrEmpty(RecordData.Instance.recordName[id]))
                {
                    RecordData.Instance.recordName[id] = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    RecordData.Instance.lastID = id;
                    RecordData.Instance.Save();
                }

                RecordData.Instance.UpdateGlobalData();

                totalPlayTimeSeconds = Mathf.FloorToInt(gameTime);
                SAVE.JsonSave(RecordData.Instance.recordName[id], ForSave(), encrypt);

                Debug.Log($"存档成功 - 槽位: {id}, 加密: {encrypt}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Save failed for slot {id}: {ex.Message}");
            }
        }


        /// <summary>
        /// 自动保存
        /// </summary>
        /// <param name="encrypt">是否加密</param>
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
                    Debug.Log($"载入成功 - 槽位: {id}, 版本: {gameVersion}");
                }
            }
            catch (Exception ex)
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
                Debug.Log($"删除存档 - 槽位: {id}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Delete failed for slot {id}: {ex.Message}");
            }
        }

        #region 辅助方法

        // 获取格式化时间
        public string GetFormattedPlayTime()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalPlayTimeSeconds);
            return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }

        // 获取最后保存时间
        public string GetFormattedLastSavedTime()
        {
            if (lastSavedTimestamp == 0) return "从未保存";
            DateTime saveTime = new DateTime(lastSavedTimestamp);
            return saveTime.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
        }

        // 验证存档完整性
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