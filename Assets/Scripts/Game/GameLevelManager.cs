using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// һ����Ϸ�ڵ�һ�����ؿ��Ĺ�����
/// </summary>
public class GameLevelManager : Singleton<GameLevelManager>
{
    private bool _levelStarted;
    public bool levelStarted => _levelStarted;

    private bool _levelFinished;
    public bool levelFinished => _levelFinished;

    private List<GameLevelData> _levelDataList;

    public void Init(List<GameLevelData> levelDataList )
    {
        _levelDataList = levelDataList;

        EnemyManager.Instance.Init();
    }

    public void InitLevel(int levelIndex)
    {
        EnemyManager.Instance.InitEnemy(_levelDataList[levelIndex].enemyDataList);
    }

    public void StartLevel()
    {
        _levelStarted = true;
    }

    public void FinishLevel()
    {
        _levelFinished = true;
    }




}
