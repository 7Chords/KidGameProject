using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI;
using UnityEngine;


namespace KidGame.Core
{
    public class EnemyHUDController : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject enemyHUDPrefab; // 敌人HUD预制件
        public Transform hudContainer;     // HUD容器
        
        [Header("Settings")]
        public Vector3 worldOffset = new Vector3(0, 2f, 0); // 世界坐标偏移
        public bool showBuffIcons = true;
        
        private Dictionary<EnemyController, EnemyHUD> activeEnemyHUDs = new Dictionary<EnemyController, EnemyHUD>();
        private Camera mainCamera;
        
        private void Start()
        {
            mainCamera = Camera.main;
        }
        
        private void Update()
        {
            foreach (var hudPair in activeEnemyHUDs)
            {
                if (hudPair.Key != null && hudPair.Value != null)
                {
                    UpdateHUDPosition(hudPair.Key.transform, hudPair.Value.transform);
                }
            }
        }
        
        public void RegisterEnemy(EnemyController enemy)
        {
            if (enemy == null || activeEnemyHUDs.ContainsKey(enemy)) return;
            
            GameObject hudObj = Instantiate(enemyHUDPrefab, hudContainer);
            EnemyHUD enemyHUD = hudObj.GetComponent<EnemyHUD>();
            
            if (enemyHUD != null)
            {
                enemyHUD.Initialize(enemy);
                activeEnemyHUDs.Add(enemy, enemyHUD);
                
                UpdateHUDPosition(enemy.transform, hudObj.transform);
                
                // 注册Buff更新
                if (showBuffIcons)
                {
                    var buffHUD = hudObj.GetComponentInChildren<BuffHUDController>();
                    if (buffHUD != null)
                    {
                        buffHUD.Initialize(enemy.GetBuffList());
                    }
                }
            }
        }
        
        public void UnregisterEnemy(EnemyController enemy)
        {
            if (enemy != null && activeEnemyHUDs.TryGetValue(enemy, out EnemyHUD hud))
            {
                Destroy(hud.gameObject);
                activeEnemyHUDs.Remove(enemy);
            }
        }
        
        private void UpdateHUDPosition(Transform enemyTransform, Transform hudTransform)
        {
            if (mainCamera == null) return;
            
            Vector3 worldPosition = enemyTransform.position + worldOffset;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

            hudTransform.position = screenPosition;
        }
        
        public void UpdateEnemySanity(EnemyController enemy, float sanity, float maxSanity)
        {
            if (enemy != null && activeEnemyHUDs.TryGetValue(enemy, out EnemyHUD hud))
            {
                hud.UpdateSanity(sanity / maxSanity);
            }
        }
        
        public void UpdateEnemyBuffs(EnemyController enemy)
        {
            if (enemy != null && activeEnemyHUDs.TryGetValue(enemy, out EnemyHUD hud))
            {
                var buffHUD = hud.GetComponentInChildren<BuffHUDController>();
                if (buffHUD != null)
                {
                    // todo. 更新Buff
                }
            }
        }
        
        private void OnDestroy()
        {
            // 清理所有HUD
            foreach (var hud in activeEnemyHUDs.Values)
            {
                if (hud != null)
                {
                    Destroy(hud.gameObject);
                }
            }
            activeEnemyHUDs.Clear();
        }
    }
}
