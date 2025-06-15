using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField]
    private List<EnemyController> enemyList;

    //≤‚ ‘
    public GameObject enemyPrefab;
    public void Init()
    {
        enemyList = new List<EnemyController>();
    }

    public void InitEnemy(List<EnemyBaseData> enemyDataList)
    {
        foreach(var data in enemyDataList)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            EnemyController enemyCtl = enemy.GetComponent<EnemyController>();
            enemyCtl.Init(data);
            enemyList.Add(enemyCtl);
        }
    }
}
