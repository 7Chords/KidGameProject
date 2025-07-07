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

        public void InitEnemy(List<EnemyBaseData> enemyDataList, List<Transform> bornPoints)
        {
            for (int i = 0; i < enemyDataList.Count; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab);
                EnemyController enemyCtl = enemy.GetComponent<EnemyController>();
                enemyList.Add(enemyCtl);
                if (i <= bornPoints.Count - 1)
                {
                    enemy.transform.position = bornPoints[i].position;
                }
                else
                {
                    enemy.transform.position = bornPoints[i % bornPoints.Count].position;
                }
                enemyCtl.Init(enemyDataList[i]);
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