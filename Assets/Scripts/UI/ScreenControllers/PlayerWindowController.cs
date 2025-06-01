using System;
using System.Collections.Generic;
using Utils;
using UnityEngine;

namespace UIFramework.Game
{
    /// <summary>
    /// PlayerWindow属性类
    /// </summary>
    [Serializable]
    public class PlayerWindowProperties : WindowProperties
    {
        public readonly List<PlayerDataEntry> PlayerData;

        public PlayerWindowProperties(List<PlayerDataEntry> data) {
            PlayerData = data;
        }
    }

    public class PlayerWindowController : WindowController<PlayerWindowProperties>
    {
        [SerializeField] 
        private LevelProgressComponent templateLevelEntry = null;
        
        private List<LevelProgressComponent> currentLevels = new List<LevelProgressComponent>();
        
        protected override void AddListeners() {
            Signals.Get<PlayerDataUpdatedSignal>().AddListener(OnDataUpdated);
        }

        protected override void RemoveListeners() {
            Signals.Get<PlayerDataUpdatedSignal>().RemoveListener(OnDataUpdated);
        }

        protected override void OnPropertiesSet() {
            OnDataUpdated(Properties.PlayerData);
        }

        private void OnDataUpdated(List<PlayerDataEntry> data) {
            VerifyElementCount(data.Count);
            RefreshElementData(data);
        }

        private void VerifyElementCount(int levelCount) {
            if (currentLevels.Count == levelCount) {
                return;
            }

            if (currentLevels.Count < levelCount) {
                while (currentLevels.Count < levelCount) {
                    var newLevel = Instantiate(templateLevelEntry, 
                        templateLevelEntry.transform.parent, 
                        false);
                    newLevel.gameObject.SetActive(true);
                    currentLevels.Add(newLevel);
                }
            }
            else {
                while (currentLevels.Count > levelCount) {
                    var levelToRemove = currentLevels[currentLevels.Count - 1];
                    currentLevels.Remove(levelToRemove);
                    Destroy(levelToRemove.gameObject);
                }
            }
        }
        
        private void RefreshElementData(List<PlayerDataEntry> playerLevelProgress) {
            for (int i = 0; i < currentLevels.Count; i++) {
                currentLevels[i].SetData(playerLevelProgress[i], i);
            }
        }
    }
}
