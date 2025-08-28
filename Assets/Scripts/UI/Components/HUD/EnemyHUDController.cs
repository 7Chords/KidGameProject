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
        public bool showBuffIcons = true;
        
        private Dictionary<EnemyController, EnemyHUD> activeEnemyHUDs = new Dictionary<EnemyController, EnemyHUD>();
        
        private void Start()
        {
            MsgCenter.RegisterMsg(MsgConst.ON_ENEMY_SANITY_CHG, OnEnemySanityChanged);
            MsgCenter.RegisterMsg(MsgConst.ON_ENEMY_BUFF_CHG, OnEnemyBuffChanged);
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
        
        private void OnEnemySanityChanged(params object[] args)
        {
            EnemyController enemy = args[0] as EnemyController;
            float curSanity = (float)args[1];
            float maxSanity = (float)args[2];

            if (enemy != null && activeEnemyHUDs.TryGetValue(enemy, out EnemyHUD hud))
            {
                hud.UpdateSanity(curSanity / maxSanity);
            }
        }

        private void OnEnemyBuffChanged(params object[] args)
        {
            EnemyController enemy = args[0] as EnemyController;

            if (enemy != null && activeEnemyHUDs.TryGetValue(enemy, out EnemyHUD hud))
            {
                var buffHUD = hud.GetComponentInChildren<BuffHUDController>();
                if (buffHUD != null)
                {
                    buffHUD.Initialize(enemy.GetBuffList());
                }
            }
        }

        private void OnDestroy()
        {
            MsgCenter.UnregisterMsg(MsgConst.ON_ENEMY_SANITY_CHG, OnEnemySanityChanged);
            MsgCenter.UnregisterMsg(MsgConst.ON_ENEMY_BUFF_CHG, OnEnemyBuffChanged);

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
