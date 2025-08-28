using System.Collections;
using System.Collections.Generic;
using KidGame.UI;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.Core
{
    public class EnemyHUD : MonoBehaviour
    {
        [Header("UI Elements")]
        public Text enemyNameText;
        public ProgressBar sanityProgressBar;
        public Transform buffsContainer;
        
        [Header("References")]
        public BuffHUDController buffHUDController;
        
        private EnemyController enemyController;
        
        public void Initialize(EnemyController enemy)
        {
            enemyController = enemy;
            
            UpdateEnemyInfo();
            
            // 初始化buff hud
            if (buffHUDController != null)
            {
                buffHUDController.Initialize(enemy.GetBuffList());
            }
        }
        
        public void UpdateSanity(float sanityPercentage)
        {
            if (sanityProgressBar != null)
            {
                sanityProgressBar.SetProgress(sanityPercentage);
            }
        }
        
        public void UpdateEnemyInfo()
        {
            if (enemyController != null && enemyNameText != null)
            {
                enemyNameText.text = enemyController.EnemyBaseData.EnemyName;
            }
        }
        
        private void OnDestroy()
        {
            // 清理资源
            enemyController = null;
        }
    }
}