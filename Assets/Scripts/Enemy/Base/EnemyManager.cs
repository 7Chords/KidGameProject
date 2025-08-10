using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        [SerializeField] private List<EnemyController> enemyList;

        //≤‚ ‘
        public GameObject enemyPrefab;

        public void Init()
        {
            enemyList = new List<EnemyController>();
        }

        public void InitEnemy(List<EnemySpawnCfg> enemySpawnCfgList)
        {
            for (int i = 0; i < enemySpawnCfgList.Count; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.transform.position = new Vector3(enemySpawnCfgList[i].enemySpawnPos.x,
                    enemySpawnCfgList[i].enemySpawnPos.y,
                    -enemySpawnCfgList[i].enemySpawnPos.z) + GameManager.Instance.GameGeneratePoint.position;
                EnemyController enemyCtl = enemy.GetComponent<EnemyController>();
                enemyList.Add(enemyCtl);
                enemyCtl.Init(enemySpawnCfgList[i].enemyData);
            }
        }

        public void DiscardEnemy()
        {
            foreach (var enemy in enemyList)
            {
                enemy.Discard();
            }
        }
    }
}