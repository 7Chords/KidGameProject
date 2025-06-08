using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家管理器 负责玩家各个模块的初始化调用 控制顺序
/// 其同样也要被GameManager初始化
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    public void Init()
    {
        PlayerUtil.Instance.Init();
        PlayerController.Instance.Init();
        PlayerBag.Instance.Init();
    }

    public void Discard()
    {
        PlayerUtil.Instance.Discard();
        PlayerController.Instance.Discard();
        PlayerBag.Instance.Discard();
    }
}