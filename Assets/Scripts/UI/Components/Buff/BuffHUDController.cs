using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class BuffHUDController : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject buffIconPrefab;
        public Transform buffIconsContainer;
        public bool showStackCount = true;
        public bool showDuration = true;
        
        private Dictionary<string, BuffIconUI> activeBuffIcons = new Dictionary<string, BuffIconUI>();
        private BuffHandler targetBuffHandler;
        
        public void Initialize(BuffHandler buffHandler)
        {
            targetBuffHandler = buffHandler;
            ClearAllIcons();
            
        }
        
        private void UpdateDisplay()
        {
            List<string> buffsToRemove = new List<string>();
            foreach (var buffId in activeBuffIcons.Keys)
            {
                bool found = false;
                foreach (var buffInfo in targetBuffHandler.buffList)
                {
                    if (buffInfo.buffData.id == buffId)
                    {
                        found = true;
                        break;
                    }
                }
                
                if (!found)
                {
                    buffsToRemove.Add(buffId);
                }
            }
            
            foreach (string buffId in buffsToRemove)
            {
                RemoveBuffIcon(buffId);
            }
            
            // 添加或更新现有buff
            foreach (var buffInfo in targetBuffHandler.buffList)
            {
                if (activeBuffIcons.ContainsKey(buffInfo.buffData.id))
                {
                    UpdateBuffIcon(buffInfo);
                }
                else
                {
                    AddBuffIcon(buffInfo);
                }
            }
        }
        
        private void AddBuffIcon(BuffInfo buffInfo)
        {
            GameObject iconObj = Instantiate(buffIconPrefab, buffIconsContainer);
            BuffIconUI buffIcon = iconObj.GetComponent<BuffIconUI>();
            
            if (buffIcon != null)
            {
                buffIcon.Initialize(buffInfo, showStackCount, showDuration);
                activeBuffIcons.Add(buffInfo.buffData.id, buffIcon);
            }
        }
        
        private void UpdateBuffIcon(BuffInfo buffInfo)
        {
            if (activeBuffIcons.TryGetValue(buffInfo.buffData.id, out BuffIconUI icon))
            {
                icon.UpdateInfo(buffInfo, showDuration);
            }
        }
        
        private void RemoveBuffIcon(string buffId)
        {
            if (activeBuffIcons.TryGetValue(buffId, out BuffIconUI icon))
            {
                Destroy(icon.gameObject);
                activeBuffIcons.Remove(buffId);
            }
        }
        
        private void ClearAllIcons()
        {
            foreach (Transform child in buffIconsContainer)
            {
                Destroy(child.gameObject);
            }
            activeBuffIcons.Clear();
        }
        
        void OnDestroy()
        {
            ClearAllIcons();
        }
    }
}