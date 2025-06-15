using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameData gameData;

    //总的游戏开始
    private bool _gameStarted;
    //总的游戏结束
    private bool _gameFinished;

    private int _levelIndex;


    //Start调用保证各个单例完成初始化
    private void Start()
    {
        InitGame();
        //临时测试 
        StartGame();
    }

    
    private void InitGame()
    {
        //游戏各个模块的初始化
        PlayerManager.Instance.Init();
        MapManager.Instance.Init();
        GameLevelManager.Instance.Init(gameData.levelDataList);
    }

    private void DiscardGame()
    {
        //游戏各个模块的销毁 主要是事件的反注册
        PlayerManager.Instance.Discard();
        MapManager.Instance.Discard();
    }

    public void StartGame()
    {
        _gameStarted = true;
        GameLevelManager.Instance.InitLevel(0);
    }

    public void FinishGame()
    {
        _gameFinished = true;
    }
}

