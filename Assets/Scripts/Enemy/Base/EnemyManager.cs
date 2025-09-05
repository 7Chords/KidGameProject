using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        [SerializeField] private List<EnemyController> enemyList;
        //测试
        public GameObject enemyPrefab;

        public void Init()
        {
            if(enemyList == null) enemyList = new List<EnemyController>();
        }

        public void Discard()
        {

        }

        public void InitEnemy(List<EnemySpawnCfg> enemySpawnCfgList)
        {
            for (int i = 0; i < enemySpawnCfgList.Count; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.transform.position = new Vector3(enemySpawnCfgList[i].enemySpawnPos.x,
                    enemySpawnCfgList[i].enemySpawnPos.y,
                    -enemySpawnCfgList[i].enemySpawnPos.z) + GameManager.Instance.GameGeneratePoint.position;

                //地图旋转导致的位置修正
                enemy.transform.RotateAround(new Vector3(GameManager.Instance.GameGeneratePoint.position.x, 
                    enemy.transform.position.y, 
                    GameManager.Instance.GameGeneratePoint.position.z),
                    Vector3.up,
                    GameManager.Instance.GameGeneratePoint.rotation.eulerAngles.y);

                EnemyController enemyCtl = enemy.GetComponent<EnemyController>();
                enemyList.Add(enemyCtl);
                enemyCtl.Init(enemySpawnCfgList[i].enemyData);
            }
            // 初始化完敌人信息

        }

        public List<EnemyController> GetAllEnemyController()
        {
            if(enemyList != null) return enemyList;

            return null;
        }
        public void DiscardEnemy()
        {
            foreach (var enemy in enemyList)
            {
                enemy.Discard();
            }
            // 应该把列表置空？
            enemyList.Clear();
        }
    }
}