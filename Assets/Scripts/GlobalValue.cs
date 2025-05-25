using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 全局用到的一些静态变量
 */
public static class GlobalValue
{
    //自己加一层 方便做暂停等功能而不互相影响其他模块
    #region TimeConst 
    public static float GameDeltaTime = Time.deltaTime;
    public static float UIDeltaTime = Time.deltaTime;
    #endregion

    #region Input
    public static bool EnablePlayerInput = true;
    #endregion
}
