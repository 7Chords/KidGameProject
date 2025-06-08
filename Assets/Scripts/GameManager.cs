using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Start调用保证各个单例完成初始化
    private void Start()
    {
        InitGame();
    }

    
    private void InitGame()
    {
        //游戏各个模块的初始化
        PlayerManager.Instance.Init();
        MapManager.Instance.Init();
    }

    private void DiscardGame()
    {
        //游戏各个模块的销毁 主要是事件的反注册
        PlayerManager.Instance.Discard();
        MapManager.Instance.Discard();
    }
}
